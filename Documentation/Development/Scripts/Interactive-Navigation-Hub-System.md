# Interactive Instruction File Navigation System

## Overview
Advanced web-based navigation system that creates an interactive, searchable, cross-referenced interface for all instruction files with real-time content analysis and intelligent recommendations.

## Features

### **Smart Navigation**
- **Contextual Recommendations**: Suggest relevant instruction files based on current task
- **Cross-Reference Mapping**: Visual graph of relationships between instruction files
- **Search with Context**: Intelligent search that understands intent, not just keywords
- **Workflow Guidance**: Step-by-step navigation through complex development processes
- **Quick Access**: Hotkey-driven navigation to frequently used instructions

### **Interactive Features**
- **Live Content Preview**: Hover to see instruction summaries
- **Bookmark System**: Save frequently accessed instruction combinations
- **Progress Tracking**: Track which instructions you've reviewed for current task
- **Annotation System**: Add personal notes and team comments to instructions
- **Version History**: Track changes and updates to instruction files

### **Intelligence Layer**
- **Usage Analytics**: Learn which instructions are most valuable for specific tasks
- **Gap Detection**: Identify missing instruction coverage for development patterns
- **Conflict Resolution**: Detect and highlight conflicting guidance between files
- **Update Notifications**: Alert when related instructions are updated

## Implementation

### **Interactive Navigation Hub: Generate-InstructionHub.ps1**
```powershell
param(
    [string]$InstructionRoot = ".github",
    [string]$OutputPath = "docs/instruction-hub",
    [switch]$IncludeAnalytics = $true
)

# Instruction file analysis engine
class InstructionFile {
    [string] $FilePath
    [string] $Name
    [string] $Category
    [string] $Title
    [string] $Summary
    [string[]] $Keywords
    [hashtable] $CrossReferences
    [int] $WordCount
    [datetime] $LastModified
    [hashtable] $Sections
    [float] $ImportanceScore
    [string[]] $RelatedFiles
    
    InstructionFile([string]$path) {
        $this.FilePath = $path
        $this.Name = (Get-Item $path).BaseName
        $this.AnalyzeContent()
        $this.CalculateImportance()
    }
    
    [void] AnalyzeContent() {
        $content = Get-Content $this.FilePath -Raw
        $this.WordCount = ($content -split '\s+').Count
        $this.LastModified = (Get-Item $this.FilePath).LastWriteTime
        
        # Extract title
        $titleMatch = [regex]::Match($content, '(?m)^#\s+(.+)$')
        $this.Title = if ($titleMatch.Success) { $titleMatch.Groups[1].Value } else { $this.Name }
        
        # Extract summary (first paragraph after title)
        $summaryMatch = [regex]::Match($content, '(?s)^#\s+.+?\n\n(.+?)\n\n')
        $this.Summary = if ($summaryMatch.Success) { $summaryMatch.Groups[1].Value.Trim() } else { "No summary available" }
        
        # Determine category from file structure
        $this.Category = $this.DetermineCategory()
        
        # Extract keywords
        $this.Keywords = $this.ExtractKeywords($content)
        
        # Extract cross-references
        $this.CrossReferences = $this.ExtractCrossReferences($content)
        
        # Extract sections
        $this.Sections = $this.ExtractSections($content)
    }
    
    [string] DetermineCategory() {
        $path = $this.FilePath
        if ($path -like "*Core-Instructions*") { return "Core" }
        if ($path -like "*UI-Instructions*") { return "UI" }
        if ($path -like "*Development-Instructions*") { return "Development" }
        if ($path -like "*Quality-Instructions*") { return "Quality" }
        if ($path -like "*Automation-Instructions*") { return "Automation" }
        return "General"
    }
    
    [string[]] ExtractKeywords([string]$content) {
        $keywords = @()
        
        # MTM-specific keywords
        if ($content -match "TransactionType") { $keywords += "TransactionType" }
        if ($content -match "ReactiveUI") { $keywords += "ReactiveUI" }
        if ($content -match "Avalonia") { $keywords += "Avalonia" }
        if ($content -match "MVVM") { $keywords += "MVVM" }
        if ($content -match "stored procedure") { $keywords += "Database" }
        if ($content -match "AddMTMServices") { $keywords += "Dependency Injection" }
        if ($content -match "naming convention") { $keywords += "Naming" }
        if ($content -match "error handling") { $keywords += "Error Handling" }
        
        return $keywords
    }
    
    [hashtable] ExtractCrossReferences([string]$content) {
        $references = @{}
        $linkMatches = [regex]::Matches($content, '\[([^\]]+)\]\(([^)]+\.instruction\.md)\)')
        
        foreach ($match in $linkMatches) {
            $references[$match.Groups[1].Value] = $match.Groups[2].Value
        }
        
        return $references
    }
    
    [hashtable] ExtractSections([string]$content) {
        $sections = @{}
        $sectionMatches = [regex]::Matches($content, '(?m)^#{2,6}\s+(.+)$')
        
        foreach ($match in $sectionMatches) {
            $sectionName = $match.Groups[1].Value.Trim()
            $sections[$sectionName] = $match.Index
        }
        
        return $sections
    }
    
    [void] CalculateImportance() {
        # Calculate importance based on various factors
        $score = 0.0
        
        # Word count factor (longer files are often more comprehensive)
        $score += [Math]::Log10($this.WordCount) * 10
        
        # Cross-reference factor (files referenced by others are more important)
        $score += $this.CrossReferences.Count * 5
        
        # Keyword relevance factor
        $criticalKeywords = @("TransactionType", "AddMTMServices", "ReactiveUI", "MVVM")
        $criticalCount = ($this.Keywords | Where-Object { $criticalKeywords -contains $_ }).Count
        $score += $criticalCount * 15
        
        # Category importance factor
        $categoryWeights = @{
            "Core" = 1.5
            "Development" = 1.3
            "Quality" = 1.2
            "UI" = 1.1
            "Automation" = 1.0
            "General" = 0.8
        }
        $score *= $categoryWeights[$this.Category]
        
        $this.ImportanceScore = $score
    }
    
    [hashtable] ToJson() {
        return @{
            name = $this.Name
            title = $this.Title
            category = $this.Category
            summary = $this.Summary
            keywords = $this.Keywords
            crossReferences = $this.CrossReferences
            wordCount = $this.WordCount
            lastModified = $this.LastModified.ToString("yyyy-MM-dd")
            sections = $this.Sections.Keys
            importanceScore = [Math]::Round($this.ImportanceScore, 2)
            filePath = $this.FilePath
        }
    }
}

# Relationship analysis engine
class InstructionRelationship {
    [string] $SourceFile
    [string] $TargetFile
    [string] $RelationshipType # "References", "Similar", "Prerequisite", "Conflicts"
    [float] $Strength
    [string] $Description
    
    InstructionRelationship([string]$source, [string]$target, [string]$type, [float]$strength) {
        $this.SourceFile = $source
        $this.TargetFile = $target
        $this.RelationshipType = $type
        $this.Strength = $strength
    }
}

Write-Host "?? Generating Interactive Instruction Navigation Hub..."

# Create output directory
if (-not (Test-Path $OutputPath)) {
    New-Item -Path $OutputPath -ItemType Directory -Force
}

# Discover and analyze all instruction files
$AllInstructions = Get-ChildItem -Path $InstructionRoot -Recurse -Filter "*.instruction.md"
$InstructionObjects = @()
$Relationships = @()

foreach ($InstructionPath in $AllInstructions) {
    Write-Host "Analyzing: $($InstructionPath.Name)"
    $InstructionObjects += [InstructionFile]::new($InstructionPath.FullName)
}

# Analyze relationships between files
foreach ($SourceInstruction in $InstructionObjects) {
    foreach ($TargetInstruction in $InstructionObjects) {
        if ($SourceInstruction.Name -eq $TargetInstruction.Name) { continue }
        
        # Check for direct references
        if ($SourceInstruction.CrossReferences.Values -contains $TargetInstruction.Name) {
            $Relationships += [InstructionRelationship]::new(
                $SourceInstruction.Name,
                $TargetInstruction.Name,
                "References",
                1.0
            )
        }
        
        # Check for keyword similarity
        $CommonKeywords = $SourceInstruction.Keywords | Where-Object { $TargetInstruction.Keywords -contains $_ }
        if ($CommonKeywords.Count -gt 0) {
            $Similarity = $CommonKeywords.Count / [Math]::Max($SourceInstruction.Keywords.Count, $TargetInstruction.Keywords.Count)
            if ($Similarity -gt 0.3) {
                $Relationships += [InstructionRelationship]::new(
                    $SourceInstruction.Name,
                    $TargetInstruction.Name,
                    "Similar",
                    $Similarity
                )
            }
        }
        
        # Check for prerequisite relationships (Core files are often prerequisites)
        if ($SourceInstruction.Category -eq "Core" -and $TargetInstruction.Category -ne "Core") {
            $Relationships += [InstructionRelationship]::new(
                $SourceInstruction.Name,
                $TargetInstruction.Name,
                "Prerequisite",
                0.7
            )
        }
    }
}

# Generate interactive HTML hub
$InstructionData = $InstructionObjects | ForEach-Object { $_.ToJson() } | ConvertTo-Json -Depth 10
$RelationshipData = $Relationships | ForEach-Object { 
    @{
        source = $_.SourceFile
        target = $_.TargetFile
        type = $_.RelationshipType
        strength = $_.Strength
    }
} | ConvertTo-Json -Depth 10

$HtmlHub = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM Instruction Navigation Hub</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #333;
            overflow-x: hidden;
        }
        
        .container { 
            max-width: 1400px; 
            margin: 0 auto; 
            background: rgba(255, 255, 255, 0.95);
            min-height: 100vh;
            backdrop-filter: blur(10px);
        }
        
        .header { 
            background: linear-gradient(135deg, #6a0dad, #4b0082); 
            color: white; 
            padding: 30px; 
            text-align: center;
            position: sticky;
            top: 0;
            z-index: 100;
            box-shadow: 0 2px 20px rgba(0,0,0,0.2);
        }
        
        .header h1 { 
            font-size: 2.5em; 
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }
        
        .search-container { 
            margin: 20px 0; 
            position: relative;
        }
        
        .search-box { 
            width: 100%; 
            max-width: 600px; 
            padding: 15px 50px 15px 20px; 
            border: none; 
            border-radius: 25px; 
            font-size: 1.1em;
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            background: rgba(255,255,255,0.9);
        }
        
        .search-icon { 
            position: absolute; 
            right: 20px; 
            top: 50%; 
            transform: translateY(-50%); 
            font-size: 1.2em;
            color: #6a0dad;
        }
        
        .nav-tabs { 
            display: flex; 
            justify-content: center; 
            margin: 20px 0; 
            border-bottom: 2px solid rgba(255,255,255,0.3);
        }
        
        .nav-tab { 
            background: rgba(255,255,255,0.2); 
            border: none; 
            padding: 12px 25px; 
            margin: 0 5px; 
            border-radius: 20px 20px 0 0; 
            color: white; 
            cursor: pointer; 
            font-size: 1em;
            transition: all 0.3s ease;
        }
        
        .nav-tab:hover, .nav-tab.active { 
            background: rgba(255,255,255,0.9); 
            color: #6a0dad;
            transform: translateY(-2px);
        }
        
        .content-area { 
            padding: 30px; 
            min-height: 600px;
        }
        
        .instruction-grid { 
            display: grid; 
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); 
            gap: 20px; 
            margin: 20px 0;
        }
        
        .instruction-card { 
            background: white; 
            border-radius: 15px; 
            padding: 20px; 
            box-shadow: 0 8px 25px rgba(0,0,0,0.1); 
            border-left: 5px solid #6a0dad;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
        }
        
        .instruction-card:hover { 
            transform: translateY(-5px); 
            box-shadow: 0 15px 35px rgba(0,0,0,0.15);
        }
        
        .instruction-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: linear-gradient(90deg, #6a0dad, #4b0082);
        }
        
        .card-header { 
            margin-bottom: 15px;
        }
        
        .card-title { 
            font-size: 1.3em; 
            font-weight: bold; 
            color: #6a0dad; 
            margin-bottom: 8px;
        }
        
        .card-category { 
            display: inline-block; 
            background: #6a0dad; 
            color: white; 
            padding: 4px 12px; 
            border-radius: 15px; 
            font-size: 0.8em; 
            margin-bottom: 10px;
        }
        
        .card-summary { 
            color: #666; 
            line-height: 1.5; 
            margin-bottom: 15px;
        }
        
        .card-keywords { 
            display: flex; 
            flex-wrap: wrap; 
            gap: 5px; 
            margin-bottom: 15px;
        }
        
        .keyword-tag { 
            background: #f0f0f0; 
            padding: 3px 8px; 
            border-radius: 12px; 
            font-size: 0.8em; 
            color: #555;
        }
        
        .card-meta { 
            font-size: 0.9em; 
            color: #888; 
            border-top: 1px solid #eee; 
            padding-top: 10px;
        }
        
        .importance-score { 
            float: right; 
            background: linear-gradient(45deg, #4caf50, #8bc34a); 
            color: white; 
            padding: 3px 8px; 
            border-radius: 10px; 
            font-size: 0.8em;
        }
        
        .relationship-graph { 
            width: 100%; 
            height: 600px; 
            border: 2px solid #ddd; 
            border-radius: 10px; 
            background: #f9f9f9;
            position: relative;
        }
        
        .quick-access { 
            position: fixed; 
            right: 20px; 
            top: 50%; 
            transform: translateY(-50%); 
            background: rgba(255,255,255,0.95); 
            padding: 20px; 
            border-radius: 15px; 
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            z-index: 90;
            max-width: 250px;
        }
        
        .quick-access h3 { 
            color: #6a0dad; 
            margin-bottom: 15px; 
            text-align: center;
        }
        
        .quick-link { 
            display: block; 
            padding: 10px; 
            margin: 5px 0; 
            background: #f8f9fa; 
            border-radius: 8px; 
            text-decoration: none; 
            color: #333; 
            transition: all 0.3s ease;
        }
        
        .quick-link:hover { 
            background: #6a0dad; 
            color: white; 
            transform: translateX(5px);
        }
        
        .filter-sidebar { 
            position: fixed; 
            left: 20px; 
            top: 50%; 
            transform: translateY(-50%); 
            background: rgba(255,255,255,0.95); 
            padding: 20px; 
            border-radius: 15px; 
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            z-index: 90;
            max-width: 200px;
        }
        
        .filter-group { 
            margin-bottom: 20px;
        }
        
        .filter-group h4 { 
            color: #6a0dad; 
            margin-bottom: 10px; 
            font-size: 0.9em;
        }
        
        .filter-option { 
            display: block; 
            margin: 5px 0;
        }
        
        .filter-option input { 
            margin-right: 8px;
        }
        
        .filter-option label { 
            font-size: 0.9em; 
            cursor: pointer;
        }
        
        .loading { 
            text-align: center; 
            padding: 50px; 
            font-size: 1.2em; 
            color: #666;
        }
        
        .stats-bar { 
            background: rgba(255,255,255,0.1); 
            padding: 15px; 
            border-radius: 10px; 
            margin: 20px 0; 
            text-align: center;
        }
        
        .stat-item { 
            display: inline-block; 
            margin: 0 20px; 
            color: white;
        }
        
        .stat-number { 
            font-size: 1.5em; 
            font-weight: bold; 
            display: block;
        }
        
        @media (max-width: 768px) {
            .quick-access, .filter-sidebar { 
                position: static; 
                transform: none; 
                margin: 20px; 
                max-width: none;
            }
            
            .instruction-grid { 
                grid-template-columns: 1fr;
            }
        }
    </style>
    <script src="https://d3js.org/d3.v7.min.js"></script>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>?? MTM Instruction Navigation Hub</h1>
            <p>Intelligent navigation for all MTM development instructions</p>
            
            <div class="stats-bar">
                <div class="stat-item">
                    <span class="stat-number" id="total-instructions">$($InstructionObjects.Count)</span>
                    <span>Instructions</span>
                </div>
                <div class="stat-item">
                    <span class="stat-number" id="total-relationships">$($Relationships.Count)</span>
                    <span>Relationships</span>
                </div>
                <div class="stat-item">
                    <span class="stat-number" id="categories">$(($InstructionObjects | Group-Object Category).Count)</span>
                    <span>Categories</span>
                </div>
            </div>
            
            <div class="search-container">
                <input type="text" class="search-box" id="searchBox" placeholder="Search instructions by keywords, categories, or content...">
                <span class="search-icon">??</span>
            </div>
            
            <div class="nav-tabs">
                <button class="nav-tab active" onclick="showTab('grid')">Grid View</button>
                <button class="nav-tab" onclick="showTab('relationships')">Relationships</button>
                <button class="nav-tab" onclick="showTab('categories')">Categories</button>
                <button class="nav-tab" onclick="showTab('workflows')">Workflows</button>
            </div>
        </div>
        
        <div class="filter-sidebar">
            <h3>?? Filters</h3>
            <div class="filter-group">
                <h4>Category</h4>
                <div class="filter-option">
                    <input type="checkbox" id="filter-core" checked>
                    <label for="filter-core">Core</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-ui" checked>
                    <label for="filter-ui">UI</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-development" checked>
                    <label for="filter-development">Development</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-quality" checked>
                    <label for="filter-quality">Quality</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-automation" checked>
                    <label for="filter-automation">Automation</label>
                </div>
            </div>
            
            <div class="filter-group">
                <h4>Keywords</h4>
                <div class="filter-option">
                    <input type="checkbox" id="filter-reactiveui">
                    <label for="filter-reactiveui">ReactiveUI</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-avalonia">
                    <label for="filter-avalonia">Avalonia</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-database">
                    <label for="filter-database">Database</label>
                </div>
                <div class="filter-option">
                    <input type="checkbox" id="filter-mvvm">
                    <label for="filter-mvvm">MVVM</label>
                </div>
            </div>
        </div>
        
        <div class="quick-access">
            <h3>? Quick Access</h3>
            <a href="#" class="quick-link" onclick="quickNavigation('core')">Core Instructions</a>
            <a href="#" class="quick-link" onclick="quickNavigation('ui')">UI Guidelines</a>
            <a href="#" class="quick-link" onclick="quickNavigation('quality')">Quality Standards</a>
            <a href="#" class="quick-link" onclick="quickNavigation('database')">Database Patterns</a>
            <a href="#" class="quick-link" onclick="quickNavigation('recent')">Recently Updated</a>
        </div>
        
        <div class="content-area">
            <div id="grid-view">
                <div class="instruction-grid" id="instructionGrid">
                    <!-- Instructions will be dynamically loaded here -->
                </div>
            </div>
            
            <div id="relationships-view" style="display: none;">
                <h2>?? Instruction Relationships</h2>
                <div class="relationship-graph" id="relationshipGraph"></div>
            </div>
            
            <div id="categories-view" style="display: none;">
                <h2>?? Category Overview</h2>
                <div id="categoryBreakdown"></div>
            </div>
            
            <div id="workflows-view" style="display: none;">
                <h2>?? Development Workflows</h2>
                <div id="workflowGuide"></div>
            </div>
        </div>
    </div>
    
    <script>
        // Instruction data loaded from PowerShell
        const instructionData = $InstructionData;
        const relationshipData = $RelationshipData;
        
        let currentFilter = {
            categories: ['Core', 'UI', 'Development', 'Quality', 'Automation'],
            keywords: [],
            searchTerm: ''
        };
        
        // Initialize the application
        document.addEventListener('DOMContentLoaded', function() {
            renderInstructionGrid();
            setupEventListeners();
            generateRelationshipGraph();
        });
        
        function setupEventListeners() {
            // Search functionality
            document.getElementById('searchBox').addEventListener('input', function(e) {
                currentFilter.searchTerm = e.target.value.toLowerCase();
                renderInstructionGrid();
            });
            
            // Filter checkboxes
            document.querySelectorAll('.filter-option input').forEach(checkbox => {
                checkbox.addEventListener('change', function() {
                    updateFilters();
                    renderInstructionGrid();
                });
            });
        }
        
        function updateFilters() {
            // Update category filters
            currentFilter.categories = [];
            ['core', 'ui', 'development', 'quality', 'automation'].forEach(category => {
                if (document.getElementById('filter-' + category).checked) {
                    currentFilter.categories.push(category.charAt(0).toUpperCase() + category.slice(1));
                }
            });
            
            // Update keyword filters
            currentFilter.keywords = [];
            ['reactiveui', 'avalonia', 'database', 'mvvm'].forEach(keyword => {
                if (document.getElementById('filter-' + keyword).checked) {
                    currentFilter.keywords.push(keyword);
                }
            });
        }
        
        function renderInstructionGrid() {
            const grid = document.getElementById('instructionGrid');
            grid.innerHTML = '';
            
            const filteredInstructions = instructionData.filter(instruction => {
                // Category filter
                if (!currentFilter.categories.includes(instruction.category)) {
                    return false;
                }
                
                // Keyword filter
                if (currentFilter.keywords.length > 0) {
                    const hasKeyword = currentFilter.keywords.some(keyword => 
                        instruction.keywords.some(instKeyword => 
                            instKeyword.toLowerCase().includes(keyword.toLowerCase())
                        )
                    );
                    if (!hasKeyword) return false;
                }
                
                // Search term filter
                if (currentFilter.searchTerm) {
                    const searchableText = (
                        instruction.title + ' ' + 
                        instruction.summary + ' ' + 
                        instruction.keywords.join(' ')
                    ).toLowerCase();
                    
                    if (!searchableText.includes(currentFilter.searchTerm)) {
                        return false;
                    }
                }
                
                return true;
            });
            
            filteredInstructions.forEach(instruction => {
                const card = createInstructionCard(instruction);
                grid.appendChild(card);
            });
            
            // Update stats
            document.getElementById('total-instructions').textContent = filteredInstructions.length;
        }
        
        function createInstructionCard(instruction) {
            const card = document.createElement('div');
            card.className = 'instruction-card';
            card.innerHTML = `
                <div class="card-header">
                    <div class="card-category">${instruction.category}</div>
                    <div class="card-title">${instruction.title}</div>
                </div>
                <div class="card-summary">${instruction.summary}</div>
                <div class="card-keywords">
                    ${instruction.keywords.map(keyword => 
                        `<span class="keyword-tag">${keyword}</span>`
                    ).join('')}
                </div>
                <div class="card-meta">
                    <span>Words: ${instruction.wordCount}</span>
                    <span> | Updated: ${instruction.lastModified}</span>
                    <span class="importance-score">${instruction.importanceScore}</span>
                </div>
            `;
            
            card.addEventListener('click', function() {
                openInstruction(instruction);
            });
            
            return card;
        }
        
        function generateRelationshipGraph() {
            const svg = d3.select("#relationshipGraph").append("svg")
                .attr("width", "100%")
                .attr("height", "100%");
            
            // Create a simple network visualization
            // (This is a simplified example - in practice you'd want a more sophisticated graph)
            const nodes = instructionData.map(d => ({id: d.name, category: d.category}));
            const links = relationshipData.map(d => ({source: d.source, target: d.target, type: d.type}));
            
            const simulation = d3.forceSimulation(nodes)
                .force("link", d3.forceLink(links).id(d => d.id))
                .force("charge", d3.forceManyBody().strength(-300))
                .force("center", d3.forceCenter(400, 300));
            
            const link = svg.append("g")
                .selectAll("line")
                .data(links)
                .enter().append("line")
                .attr("stroke", "#999")
                .attr("stroke-opacity", 0.6);
            
            const node = svg.append("g")
                .selectAll("circle")
                .data(nodes)
                .enter().append("circle")
                .attr("r", 8)
                .attr("fill", d => getCategoryColor(d.category))
                .call(d3.drag()
                    .on("start", dragstarted)
                    .on("drag", dragged)
                    .on("end", dragended));
            
            node.append("title")
                .text(d => d.id);
            
            simulation.on("tick", () => {
                link
                    .attr("x1", d => d.source.x)
                    .attr("y1", d => d.source.y)
                    .attr("x2", d => d.target.x)
                    .attr("y2", d => d.target.y);
                
                node
                    .attr("cx", d => d.x)
                    .attr("cy", d => d.y);
            });
            
            function dragstarted(event, d) {
                if (!event.active) simulation.alphaTarget(0.3).restart();
                d.fx = d.x;
                d.fy = d.y;
            }
            
            function dragged(event, d) {
                d.fx = event.x;
                d.fy = event.y;
            }
            
            function dragended(event, d) {
                if (!event.active) simulation.alphaTarget(0);
                d.fx = null;
                d.fy = null;
            }
        }
        
        function getCategoryColor(category) {
            const colors = {
                'Core': '#6a0dad',
                'UI': '#4caf50',
                'Development': '#ff9800',
                'Quality': '#f44336',
                'Automation': '#2196f3'
            };
            return colors[category] || '#999';
        }
        
        function showTab(tabName) {
            // Hide all views
            document.querySelectorAll('#grid-view, #relationships-view, #categories-view, #workflows-view').forEach(view => {
                view.style.display = 'none';
            });
            
            // Show selected view
            document.getElementById(tabName + '-view').style.display = 'block';
            
            // Update tab styling
            document.querySelectorAll('.nav-tab').forEach(tab => {
                tab.classList.remove('active');
            });
            event.target.classList.add('active');
        }
        
        function quickNavigation(type) {
            switch(type) {
                case 'core':
                    currentFilter.categories = ['Core'];
                    break;
                case 'ui':
                    currentFilter.categories = ['UI'];
                    break;
                case 'quality':
                    currentFilter.categories = ['Quality'];
                    break;
                case 'database':
                    currentFilter.keywords = ['database'];
                    break;
                case 'recent':
                    // Sort by last modified and show recent
                    instructionData.sort((a, b) => new Date(b.lastModified) - new Date(a.lastModified));
                    break;
            }
            
            renderInstructionGrid();
            showTab('grid');
        }
        
        function openInstruction(instruction) {
            // Open the instruction file (would integrate with file system or web server)
            alert('Opening: ' + instruction.title + '\nFile: ' + instruction.filePath);
        }
    </script>
</body>
</html>
"@

# Save the hub
$HubPath = Join-Path $OutputPath "index.html"
$HtmlHub | Out-File -FilePath $HubPath -Encoding UTF8

Write-Host "? Interactive Instruction Navigation Hub generated: $HubPath"
Write-Host "?? Open in browser: file:///$($HubPath.Replace('\', '/'))"

# Generate supporting files
$CssPath = Join-Path $OutputPath "styles.css"
$JsPath = Join-Path $OutputPath "navigation.js"

# Additional CSS for advanced styling
$AdvancedCss = @"
/* Advanced animations and transitions */
@keyframes fadeInUp {
    from { opacity: 0; transform: translateY(30px); }
    to { opacity: 1; transform: translateY(0); }
}

.instruction-card {
    animation: fadeInUp 0.5s ease-out;
}

/* Responsive breakpoints */
@media (max-width: 1200px) {
    .instruction-grid { grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); }
}

@media (max-width: 992px) {
    .instruction-grid { grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); }
    .filter-sidebar, .quick-access { position: relative; width: 100%; margin: 20px 0; }
}

@media (max-width: 768px) {
    .header h1 { font-size: 2em; }
    .instruction-grid { grid-template-columns: 1fr; }
    .nav-tabs { flex-direction: column; }
    .nav-tab { margin: 2px 0; }
}

/* Print styles */
@media print {
    .filter-sidebar, .quick-access, .nav-tabs { display: none; }
    .instruction-card { break-inside: avoid; }
}
"@

$AdvancedCss | Out-File -FilePath $CssPath -Encoding UTF8

Write-Host "?? Supporting files generated in: $OutputPath"
Write-Host "?? CSS: $CssPath"
Write-Host "?? Data: $($InstructionObjects.Count) instructions analyzed"
Write-Host "?? Relationships: $($Relationships.Count) connections found"
```