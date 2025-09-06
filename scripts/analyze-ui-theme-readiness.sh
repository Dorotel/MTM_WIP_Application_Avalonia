#!/bin/bash

# UI Theme Readiness Analysis Script (Bash Version)
# Systematically analyzes all AXAML view files for theme compliance

REPO_ROOT="/home/runner/work/MTM_WIP_Application_Avalonia/MTM_WIP_Application_Avalonia"
VIEWS_PATH="$REPO_ROOT/Views"
REPORTS_PATH="$REPO_ROOT/docs/uithemereadyness"
CURRENT_DATE=$(date "+%Y-%m-%d %H:%M:%S")

# Create reports directory
mkdir -p "$REPORTS_PATH"

echo "🎯 MTM UI Theme Readiness Analysis"
echo "================================================"

# Find all AXAML files
AXAML_FILES=$(find "$VIEWS_PATH" -name "*.axaml" -type f | grep -v -E "\.(backup|resource-fix-backup)\." | sort)
TOTAL_FILES=$(echo "$AXAML_FILES" | wc -l)

echo "📁 Found $TOTAL_FILES view files to analyze"
echo ""

# Initialize counters
COMPLIANT_COUNT=0
NEEDS_WORK_COUNT=0
PENDING_COUNT=0
TOTAL_ISSUES=0
TOTAL_HARDCODED_COLORS=0

# Initialize summary
SUMMARY_CONTENT="# UI Theme Readiness Analysis Summary

**Analysis Date**: $CURRENT_DATE  
**Total View Files Analyzed**: $TOTAL_FILES

## 📊 Individual File Analysis

"

# Process each file
while IFS= read -r file_path; do
    if [ -z "$file_path" ]; then
        continue
    fi
    
    filename=$(basename "$file_path")
    basename_no_ext="${filename%.axaml}"
    relative_path="${file_path#$REPO_ROOT/}"
    
    echo "🔍 Analyzing: $filename"
    
    # Read file content
    if [ ! -f "$file_path" ]; then
        echo "❌ File not found: $file_path"
        continue
    fi
    
    content=$(cat "$file_path")
    
    # Initialize analysis variables
    hardcoded_colors=0
    theme_resources=0
    issues=()
    recommendations=()
    compliance_score=0
    
    # Check for hardcoded colors
    hex_colors=$(echo "$content" | grep -oE "#[A-Fa-f0-9]{6}|#[A-Fa-f0-9]{8}" | wc -l)
    named_colors=$(echo "$content" | grep -oE 'Color="(Red|Blue|Green|Yellow|Orange|Purple|Pink|Brown|Gray|Grey|Black|White|Silver|Gold)"' | wc -l)
    
    hardcoded_colors=$((hex_colors + named_colors))
    
    if [ $hardcoded_colors -gt 0 ]; then
        issues+=("$hardcoded_colors hardcoded colors found")
        recommendations+=("Replace hardcoded colors with MTM theme resources")
    fi
    
    # Check for theme resource usage
    theme_resources=$(echo "$content" | grep -oE "\{DynamicResource\s+MTM_Shared_Logic\.[^}]+" | wc -l)
    
    # Check UI guidelines
    ui_issues=0
    
    # Check namespace
    if ! echo "$content" | grep -q 'xmlns="https://github.com/avaloniaui"'; then
        issues+=("Missing or incorrect Avalonia namespace")
        recommendations+=("Add correct Avalonia namespace")
        ui_issues=$((ui_issues + 1))
    fi
    
    # Check for Name vs x:Name on Grid
    if echo "$content" | grep -q 'Name="[^"]*Grid[^"]*"'; then
        issues+=("Grid uses 'Name' instead of 'x:Name' (AVLN2000 violation)")
        recommendations+=("Replace Grid Name= with x:Name=")
        ui_issues=$((ui_issues + 1))
    fi
    
    # Check for tab view pattern (for tab view files)
    if echo "$filename" | grep -q "TabView\.axaml"; then
        if ! echo "$content" | grep -q -E '<ScrollViewer[^>]*>.*<Grid[^>]*RowDefinitions="\*,Auto"'; then
            issues+=("Missing mandatory Tab View pattern (ScrollViewer + Grid)")
            recommendations+=("Implement Tab View pattern: ScrollViewer + Grid with RowDefinitions=\"*,Auto\"")
            ui_issues=$((ui_issues + 1))
        fi
    fi
    
    # Calculate compliance score
    total_checks=10
    passed_checks=0
    
    # Theme compliance (40% weight)
    if [ $hardcoded_colors -eq 0 ]; then
        passed_checks=$((passed_checks + 2))
    fi
    if [ $theme_resources -gt 0 ]; then
        passed_checks=$((passed_checks + 2))
    fi
    
    # UI guidelines (40% weight)
    if [ $ui_issues -eq 0 ]; then
        passed_checks=$((passed_checks + 4))
    elif [ $ui_issues -eq 1 ]; then
        passed_checks=$((passed_checks + 2))
    fi
    
    # General quality (20% weight)
    total_issues_count=${#issues[@]}
    if [ $total_issues_count -eq 0 ]; then
        passed_checks=$((passed_checks + 2))
    elif [ $total_issues_count -le 2 ]; then
        passed_checks=$((passed_checks + 1))
    fi
    
    compliance_score=$(( (passed_checks * 100) / total_checks ))
    
    # Determine status
    if [ $compliance_score -ge 90 ]; then
        status="COMPLIANT"
        status_color="\033[32m" # Green
        COMPLIANT_COUNT=$((COMPLIANT_COUNT + 1))
    elif [ $compliance_score -ge 70 ]; then
        status="NEEDS-WORK"
        status_color="\033[33m" # Yellow
        NEEDS_WORK_COUNT=$((NEEDS_WORK_COUNT + 1))
    else
        status="PENDING"
        status_color="\033[31m" # Red
        PENDING_COUNT=$((PENDING_COUNT + 1))
    fi
    
    echo -e "  ${status_color}${compliance_score}% compliant ($status)\033[0m"
    
    # Update totals
    TOTAL_ISSUES=$((TOTAL_ISSUES + total_issues_count))
    TOTAL_HARDCODED_COLORS=$((TOTAL_HARDCODED_COLORS + hardcoded_colors))
    
    # Generate individual report
    REPORT_FILE="$REPORTS_PATH/${basename_no_ext}_theme_readiness_checklist.md"
    
    # Create report content
    REPORT_CONTENT="# UI Theme Readiness Checklist

**View File**: \`$filename\`  
**File Path**: \`$relative_path\`  
**Analysis Date**: $CURRENT_DATE  
**Analyst**: Automated Analysis  

## 🎯 Analysis Results

### ✅ Compliance Score: ${compliance_score}%
- **Status**: $status
- **Theme Resources Used**: $theme_resources
- **Hardcoded Colors Found**: $hardcoded_colors
- **Total Issues**: $total_issues_count

### 📊 Detailed Findings

#### 🎨 Theme Resource Usage"

    if [ $theme_resources -gt 0 ]; then
        REPORT_CONTENT+="\n✅ **Theme Resources Found**: $theme_resources MTM theme resources in use"
        
        # List theme resources found
        theme_resource_list=$(echo "$content" | grep -oE "\{DynamicResource\s+MTM_Shared_Logic\.[^}]+" | sort | uniq)
        if [ ! -z "$theme_resource_list" ]; then
            REPORT_CONTENT+="\n\n**Theme Resources Used:**"
            while IFS= read -r resource; do
                clean_resource=$(echo "$resource" | sed 's/{DynamicResource\s*//' | sed 's/}//')
                REPORT_CONTENT+="\n- $clean_resource"
            done <<< "$theme_resource_list"
        fi
    else
        REPORT_CONTENT+="\n⚠️ **No MTM theme resources found** - File may be using hardcoded colors or non-MTM resources"
    fi

    REPORT_CONTENT+="\n\n#### 🚫 Hardcoded Color Validation"
    
    if [ $hardcoded_colors -eq 0 ]; then
        REPORT_CONTENT+="\n✅ **No hardcoded colors found** - Excellent theme compliance!"
    else
        REPORT_CONTENT+="\n❌ **Hardcoded colors detected**: $hardcoded_colors violations found"
        
        if [ $hex_colors -gt 0 ]; then
            REPORT_CONTENT+="\n- Hex colors: $hex_colors found"
            hex_list=$(echo "$content" | grep -oE "#[A-Fa-f0-9]{6}|#[A-Fa-f0-9]{8}" | sort | uniq)
            while IFS= read -r hex_color; do
                if [ ! -z "$hex_color" ]; then
                    REPORT_CONTENT+="\n  - \`$hex_color\`"
                fi
            done <<< "$hex_list"
        fi
        
        if [ $named_colors -gt 0 ]; then
            REPORT_CONTENT+="\n- Named colors: $named_colors found"
        fi
    fi

    REPORT_CONTENT+="\n\n### ⚠️ Issues Identified"
    
    if [ ${#issues[@]} -eq 0 ]; then
        REPORT_CONTENT+="\n✅ **No issues found** - This view meets all analyzed criteria!"
    else
        for issue in "${issues[@]}"; do
            REPORT_CONTENT+="\n- ❌ $issue"
        done
    fi

    REPORT_CONTENT+="\n\n### 🔧 Recommendations"
    
    if [ ${#recommendations[@]} -eq 0 ]; then
        REPORT_CONTENT+="\n✅ **No immediate actions required** - This view is well-structured!"
    else
        for rec in "${recommendations[@]}"; do
            REPORT_CONTENT+="\n- 🔧 $rec"
        done
    fi

    REPORT_CONTENT+="\n\n## 📋 MTM UI Guidelines Checklist

### ✅ Theme Compliance
- [$([ $hardcoded_colors -eq 0 ] && echo "x" || echo " ")] No hardcoded colors (hex or named)
- [$([ $theme_resources -gt 0 ] && echo "x" || echo " ")] Uses MTM theme resources (\`MTM_Shared_Logic.*\`)
- [ ] All text elements use appropriate semantic colors
- [ ] All interactive elements follow MTM color scheme

### ✅ Layout Standards  
- [$(echo "$content" | grep -q 'xmlns="https://github.com/avaloniaui"' && echo "x" || echo " ")] Correct Avalonia namespace
- [$(echo "$content" | grep -q 'x:Name="[^"]*Grid[^"]*"' && echo "x" || echo " ")] Grid uses x:Name (not Name)
- [ ] Consistent spacing (8px, 16px, 24px)
- [ ] Card-based layout where appropriate"

    if echo "$filename" | grep -q "TabView\.axaml"; then
        REPORT_CONTENT+="\n- [$(echo "$content" | grep -q -E '<ScrollViewer[^>]*>.*<Grid[^>]*RowDefinitions="\*,Auto"' && echo "x" || echo " ")] Tab View pattern implemented (ScrollViewer + Grid)"
    fi

    REPORT_CONTENT+="\n\n### ✅ WCAG 2.1 AA Compliance
- [ ] Text contrast ratios ≥ 4.5:1
- [ ] Interactive elements have focus indicators  
- [ ] Color is not the only way to convey information
- [ ] Works in high contrast themes

## 🎯 Next Steps

Based on the analysis, the next priority actions are:
"

    if [ ${#recommendations[@]} -gt 0 ]; then
        step_counter=1
        for rec in "${recommendations[@]}"; do
            REPORT_CONTENT+="\n${step_counter}. $rec"
            step_counter=$((step_counter + 1))
        done
    else
        REPORT_CONTENT+="\n1. ✅ This view meets current analysis criteria
2. Manual validation recommended for WCAG contrast ratios
3. Test theme switching to ensure proper visual appearance"
    fi

    REPORT_CONTENT+="\n\n**Status**: $status  
**Overall Compliance**: ${compliance_score}%  
**Generated**: $CURRENT_DATE"
    
    # Write report to file
    echo -e "$REPORT_CONTENT" > "$REPORT_FILE"
    echo "📄 Generated: ${basename_no_ext}_theme_readiness_checklist.md"
    
    # Add to summary
    SUMMARY_CONTENT+="### $filename
- **File**: \`$relative_path\`
- **Status**: $status (${compliance_score}% compliant)
- **Theme Resources**: $theme_resources used
- **Hardcoded Colors**: $hardcoded_colors found
- **Issues**: $total_issues_count identified
- **Report**: [${basename_no_ext}_theme_readiness_checklist.md](./${basename_no_ext}_theme_readiness_checklist.md)

"

done <<< "$AXAML_FILES"

# Complete summary
SUMMARY_CONTENT+="
## 📊 Overall Results

### Compliance Status Distribution
- ✅ **Compliant Files (90%+)**: $COMPLIANT_COUNT files
- ⚠️ **Needs Work Files (70-89%)**: $NEEDS_WORK_COUNT files  
- 🔄 **Pending Files (<70%)**: $PENDING_COUNT files

### Issue Summary
- 🎨 **Total Issues Found**: $TOTAL_ISSUES
- 🚫 **Total Hardcoded Colors**: $TOTAL_HARDCODED_COLORS
- 📈 **Average Compliance**: $(( (COMPLIANT_COUNT * 100 + NEEDS_WORK_COUNT * 80 + PENDING_COUNT * 50) / TOTAL_FILES ))% (estimated)

## 🎯 Priority Actions

### High Priority (Pending Files)
Files with compliance < 70% should be addressed first:
- Focus on eliminating hardcoded colors
- Implement proper MTM theme resource usage
- Fix critical UI guideline violations

### Medium Priority (Needs Work Files)  
Files with 70-89% compliance need refinement:
- Fine-tune theme resource usage
- Address remaining guideline violations
- Validate WCAG contrast ratios

### Maintenance (Compliant Files)
Files with 90%+ compliance:
- Manual WCAG validation recommended
- Cross-theme testing
- Performance validation

## 🛠️ Tools and Resources

- **Template**: [UI-THEME-READINESS-CHECKLIST-TEMPLATE.md](./UI-THEME-READINESS-CHECKLIST-TEMPLATE.md)
- **MTM Theme Resources**: See \`Resources/Themes/MTMTheme.axaml\` for complete brush definitions
- **UI Guidelines**: Refer to MTM custom instructions for layout patterns

---

**Generated by**: MTM UI Theme Readiness Analysis Tool  
**Analysis Date**: $CURRENT_DATE  
**Total Files**: $TOTAL_FILES  
**Analysis Status**: Complete ✅
"

# Write summary
echo -e "$SUMMARY_CONTENT" > "$REPORTS_PATH/ANALYSIS-SUMMARY.md"

echo ""
echo "📊 Analysis Complete!"
echo "================================================"
echo "Total Files: $TOTAL_FILES"
echo "✅ Compliant: $COMPLIANT_COUNT"
echo "⚠️  Needs Work: $NEEDS_WORK_COUNT" 
echo "🔄 Pending: $PENDING_COUNT"
echo "🎨 Total Issues: $TOTAL_ISSUES"
echo "🚫 Hardcoded Colors: $TOTAL_HARDCODED_COLORS"
echo ""
echo "📁 Reports generated in: $REPORTS_PATH"
echo "📄 Summary report: ANALYSIS-SUMMARY.md"
echo ""
echo "🎯 UI Theme Readiness Analysis Complete!"