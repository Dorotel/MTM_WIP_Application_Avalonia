#!/usr/bin/env node

/**
 * MTM Documentation Link Validator
 * Automated link validation system for .github/ and docs/ directories
 * Generates comprehensive link health reports for maintenance
 */

const fs = require('fs');
const path = require('path');

class MTMDocumentationLinkValidator {
    constructor(basePath = '.') {
        this.basePath = basePath;
        this.brokenLinks = [];
        this.validatedLinks = new Set();
        this.linkMap = new Map();
        this.stats = {
            totalFiles: 0,
            totalLinks: 0,
            brokenLinks: 0,
            validLinks: 0
        };
    }

    /**
     * Main validation method - scans all documentation and validates links
     */
    async validateAllDocumentation() {
        console.log('🔍 Starting MTM Documentation Link Validation...');
        
        // Scan .github/ directory
        const githubFiles = this.scanMarkdownFiles(path.join(this.basePath, '.github'));
        console.log(`📁 Found ${githubFiles.length} files in .github/`);
        
        // Scan docs/ directory
        const docsFiles = this.scanMarkdownFiles(path.join(this.basePath, 'docs'));
        console.log(`📁 Found ${docsFiles.length} files in docs/`);
        
        const allFiles = [...githubFiles, ...docsFiles];
        this.stats.totalFiles = allFiles.length;
        
        // Extract all links from all files
        console.log('🔗 Extracting links from all files...');
        for (const file of allFiles) {
            this.extractLinksFromFile(file);
        }
        
        console.log(`🔗 Found ${this.stats.totalLinks} total links to validate`);
        
        // Validate all extracted links
        console.log('✅ Validating link targets...');
        this.validateAllLinks();
        
        // Generate comprehensive report
        return this.generateReport();
    }

    /**
     * Recursively scan directory for markdown files
     */
    scanMarkdownFiles(directory) {
        if (!fs.existsSync(directory)) {
            return [];
        }

        const files = [];
        
        const scanRecursive = (dir) => {
            const items = fs.readdirSync(dir);
            
            for (const item of items) {
                const itemPath = path.join(dir, item);
                const stat = fs.statSync(itemPath);
                
                if (stat.isDirectory()) {
                    // Skip node_modules, .git, and other system directories
                    if (!item.startsWith('.') && item !== 'node_modules') {
                        scanRecursive(itemPath);
                    }
                } else if (item.endsWith('.md')) {
                    files.push(itemPath);
                }
            }
        };
        
        scanRecursive(directory);
        return files;
    }

    /**
     * Extract all markdown links from a file
     */
    extractLinksFromFile(filePath) {
        try {
            const content = fs.readFileSync(filePath, 'utf8');
            
            // Regex patterns for different link types
            const patterns = [
                // Standard markdown links: [text](link)
                /\[([^\]]*)\]\(([^)]+)\)/g,
                // Reference links: [text][ref] and [ref]: url
                /\[([^\]]*)\]\[([^\]]+)\]/g,
                /^\[([^\]]+)\]:\s*(.+)$/gm,
                // HTML links: <a href="url">
                /<a[^>]+href=["']([^"']+)["'][^>]*>/g,
                // Direct URLs (optional - might be too aggressive)
                // /https?:\/\/[^\s\)]+/g
            ];
            
            patterns.forEach(pattern => {
                let match;
                while ((match = pattern.exec(content)) !== null) {
                    let linkUrl = match[2] || match[1]; // Different capture groups for different patterns
                    
                    // Skip empty links
                    if (!linkUrl || linkUrl.trim() === '') continue;
                    
                    // Skip external links (http/https)
                    if (linkUrl.startsWith('http://') || linkUrl.startsWith('https://')) continue;
                    
                    // Skip mailto and other protocols
                    if (linkUrl.includes(':') && !linkUrl.startsWith('./') && !linkUrl.startsWith('../') && !linkUrl.startsWith('/')) continue;
                    
                    // Skip anchor links within same document
                    if (linkUrl.startsWith('#')) continue;
                    
                    // Clean up the link
                    linkUrl = linkUrl.trim();
                    
                    // Remove anchor fragments for file existence checking
                    const cleanUrl = linkUrl.split('#')[0];
                    
                    if (cleanUrl) {
                        this.linkMap.set(`${filePath}::${linkUrl}`, {
                            sourceFile: filePath,
                            originalLink: linkUrl,
                            cleanLink: cleanUrl,
                            lineNumber: this.findLineNumber(content, match.index)
                        });
                        this.stats.totalLinks++;
                    }
                }
            });
        } catch (error) {
            console.warn(`⚠️ Warning: Could not read file ${filePath}: ${error.message}`);
        }
    }

    /**
     * Find line number for a given character index in content
     */
    findLineNumber(content, index) {
        const lines = content.substring(0, index).split('\n');
        return lines.length;
    }

    /**
     * Validate all extracted links
     */
    validateAllLinks() {
        for (const [key, linkInfo] of this.linkMap.entries()) {
            const isValid = this.validateSingleLink(linkInfo);
            
            if (isValid) {
                this.stats.validLinks++;
                this.validatedLinks.add(key);
            } else {
                this.stats.brokenLinks++;
                this.brokenLinks.push(linkInfo);
            }
        }
    }

    /**
     * Validate a single link
     */
    validateSingleLink(linkInfo) {
        const { sourceFile, cleanLink } = linkInfo;
        
        // Resolve relative path
        let targetPath;
        
        if (cleanLink.startsWith('/')) {
            // Absolute path from repository root
            targetPath = path.join(this.basePath, cleanLink.substring(1));
        } else {
            // Relative path from source file directory
            const sourceDir = path.dirname(sourceFile);
            targetPath = path.resolve(sourceDir, cleanLink);
        }
        
        // Normalize path separators
        targetPath = path.normalize(targetPath);
        
        // Check if file or directory exists
        try {
            return fs.existsSync(targetPath);
        } catch (error) {
            return false;
        }
    }

    /**
     * Generate comprehensive validation report
     */
    generateReport() {
        const healthScore = this.stats.totalLinks > 0 
            ? Math.round((this.stats.validLinks / this.stats.totalLinks) * 100)
            : 100;

        const report = {
            timestamp: new Date().toISOString(),
            summary: {
                totalFiles: this.stats.totalFiles,
                totalLinks: this.stats.totalLinks,
                validLinks: this.stats.validLinks,
                brokenLinks: this.stats.brokenLinks,
                healthScore: healthScore
            },
            brokenLinks: this.brokenLinks.map(link => ({
                sourceFile: link.sourceFile.replace(this.basePath, '').replace(/^[\/\\]/, ''),
                targetLink: link.originalLink,
                lineNumber: link.lineNumber,
                resolvedPath: this.resolveTargetPath(link)
            })),
            recommendations: this.generateRecommendations()
        };

        // Write detailed report
        this.writeDetailedReport(report);
        
        // Print console summary
        this.printConsoleSummary(report);
        
        return report;
    }

    /**
     * Resolve the target path for reporting
     */
    resolveTargetPath(linkInfo) {
        const { sourceFile, cleanLink } = linkInfo;
        
        if (cleanLink.startsWith('/')) {
            return path.join(this.basePath, cleanLink.substring(1));
        } else {
            const sourceDir = path.dirname(sourceFile);
            return path.resolve(sourceDir, cleanLink);
        }
    }

    /**
     * Generate recommendations based on validation results
     */
    generateRecommendations() {
        const recommendations = [];
        
        if (this.stats.brokenLinks > 0) {
            recommendations.push('Review and fix broken links listed in the detailed report');
            recommendations.push('Consider using relative paths for internal documentation references');
            recommendations.push('Verify file paths are correct after any documentation restructuring');
        }
        
        if (this.stats.brokenLinks > this.stats.totalLinks * 0.1) {
            recommendations.push('High number of broken links detected - consider comprehensive link audit');
        }
        
        if (this.stats.totalLinks === 0) {
            recommendations.push('No internal links found - consider adding cross-references to improve navigation');
        }
        
        recommendations.push('Run link validation regularly as part of documentation maintenance');
        recommendations.push('Update this validator if new link patterns are introduced');
        
        return recommendations;
    }

    /**
     * Write detailed report to file
     */
    writeDetailedReport(report) {
        const auditDir = path.join(this.basePath, 'docs', 'audit');
        
        // Ensure audit directory exists
        if (!fs.existsSync(auditDir)) {
            fs.mkdirSync(auditDir, { recursive: true });
        }
        
        const reportPath = path.join(auditDir, 'link-validation-report.md');
        
        const markdown = this.generateMarkdownReport(report);
        fs.writeFileSync(reportPath, markdown, 'utf8');
        
        console.log(`📄 Detailed report written to: ${reportPath}`);
    }

    /**
     * Generate markdown format report
     */
    generateMarkdownReport(report) {
        const { summary, brokenLinks, recommendations } = report;
        
        let markdown = `# MTM Documentation Link Validation Report

**Generated**: ${report.timestamp}  
**Health Score**: ${summary.healthScore}%  

## Summary

- **Total Files Scanned**: ${summary.totalFiles}
- **Total Links Found**: ${summary.totalLinks}
- **Valid Links**: ${summary.validLinks}
- **Broken Links**: ${summary.brokenLinks}

`;

        if (brokenLinks.length > 0) {
            markdown += `## 🚨 Broken Links

| Source File | Target Link | Line | Issue |
|-------------|-------------|------|-------|
`;
            
            brokenLinks.forEach(link => {
                markdown += `| \`${link.sourceFile}\` | \`${link.targetLink}\` | ${link.lineNumber} | File not found |\n`;
            });
            
            markdown += '\n';
        } else {
            markdown += `## ✅ All Links Valid

No broken links detected in the documentation.

`;
        }

        markdown += `## 📋 Recommendations

`;
        recommendations.forEach(rec => {
            markdown += `- ${rec}\n`;
        });

        markdown += `
## Validation Details

This report was generated by the MTM Documentation Link Validator, which scans:
- All markdown files in \`.github/\` directory
- All markdown files in \`docs/\` directory
- Internal links only (external HTTP/HTTPS links are not validated)
- Relative and absolute paths within the repository

## Next Steps

1. **Fix Broken Links**: Address any broken links listed above
2. **Regular Validation**: Run this validator after documentation changes
3. **Automation**: Consider integrating into CI/CD pipeline for automatic validation
4. **Maintenance**: Update link patterns as documentation structure evolves

---

*Generated by MTM Documentation Link Validator v1.0*
`;

        return markdown;
    }

    /**
     * Print console summary
     */
    printConsoleSummary(report) {
        const { summary, brokenLinks } = report;
        
        console.log('\n📊 Link Validation Summary:');
        console.log(`   Files Scanned: ${summary.totalFiles}`);
        console.log(`   Links Found: ${summary.totalLinks}`);
        console.log(`   Valid Links: ${summary.validLinks}`);
        console.log(`   Broken Links: ${summary.brokenLinks}`);
        console.log(`   Health Score: ${summary.healthScore}%`);
        
        if (brokenLinks.length > 0) {
            console.log('\n🚨 Broken Links Found:');
            brokenLinks.forEach(link => {
                console.log(`   ${link.sourceFile}:${link.lineNumber} -> ${link.targetLink}`);
            });
        } else {
            console.log('\n✅ All links are valid!');
        }
        
        console.log('\n📄 Detailed report available in docs/audit/link-validation-report.md');
    }
}

// CLI execution
if (require.main === module) {
    const validator = new MTMDocumentationLinkValidator();
    
    validator.validateAllDocumentation()
        .then(report => {
            process.exit(report.summary.brokenLinks > 0 ? 1 : 0);
        })
        .catch(error => {
            console.error('❌ Link validation failed:', error);
            process.exit(1);
        });
}

module.exports = MTMDocumentationLinkValidator;