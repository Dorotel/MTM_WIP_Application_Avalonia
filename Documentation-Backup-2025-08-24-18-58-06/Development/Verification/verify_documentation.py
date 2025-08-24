#!/usr/bin/env python3
"""
MTM Documentation Accuracy Verification Script
Automated verification system for README and HTML files accuracy
"""

import os
import re
import json
import argparse
from pathlib import Path
from datetime import datetime
import subprocess

class MTMDocumentationVerifier:
    def __init__(self, repo_path):
        self.repo_path = Path(repo_path)
        self.verification_results = {
            "verificationDate": datetime.now().isoformat(),
            "overallScore": 0.0,
            "criticalIssues": 0,
            "highPriorityIssues": 0,
            "mediumPriorityIssues": 0,
            "lowPriorityIssues": 0,
            "fileResults": [],
            "automatedFixPrompts": []
        }
        
        # MTM Validation Rules
        self.mtm_validation_rules = {
            "TransactionType": {
                "pattern": "TransactionType is determined by USER INTENT",
                "antiPatterns": [
                    "operation switch",
                    "operation === \"90\"",
                    "GetTransactionType\\(operation\\)"
                ],
                "severity": "Critical"
            },
            "OperationNumbers": {
                "pattern": "string workflow identifiers",
                "validFormats": ["\"90\"", "\"100\"", "\"110\""],
                "invalidFormats": ["90", "100", "110"],
                "severity": "High"
            },
            "ColorScheme": {
                "requiredColors": ["#4B45ED", "#BA45ED", "#8345ED", "#4574ED", "#ED45E7", "#B594ED"],
                "severity": "Medium"
            },
            "AddMTMServices": {
                "pattern": "AddMTMServices\\(configuration\\)",
                "antiPatterns": [
                    "AddCoreServices",
                    "AddBusinessServices",
                    "AddUtilityServices"
                ],
                "severity": "Critical"
            }
        }

    def verify_readme_files(self):
        """Verify all README files for accuracy"""
        readme_files = list(self.repo_path.glob("**/README*.md"))
        print(f"Found {len(readme_files)} README files")
        
        for readme_file in readme_files:
            print(f"Verifying: {readme_file.relative_to(self.repo_path)}")
            result = self.verify_single_readme(readme_file)
            self.verification_results["fileResults"].append(result)
        
        return self.verification_results

    def verify_single_readme(self, file_path):
        """Verify a single README file"""
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        issues = []
        accuracy_score = 100.0
        
        # Check for MTM business logic compliance
        issues.extend(self.check_mtm_compliance(content, str(file_path)))
        
        # Check for .NET 8 framework compliance
        issues.extend(self.check_framework_compliance(content, str(file_path)))
        
        # Check for broken cross-references
        issues.extend(self.check_cross_references(content, file_path))
        
        # Check for AddMTMServices accuracy
        issues.extend(self.check_service_registration(content, str(file_path)))
        
        # Calculate accuracy score
        critical_issues = len([i for i in issues if i["severity"] == "Critical"])
        high_issues = len([i for i in issues if i["severity"] == "High"])
        medium_issues = len([i for i in issues if i["severity"] == "Medium"])
        
        accuracy_score -= (critical_issues * 25) + (high_issues * 10) + (medium_issues * 5)
        accuracy_score = max(0, accuracy_score)
        
        # Update overall counters
        self.verification_results["criticalIssues"] += critical_issues
        self.verification_results["highPriorityIssues"] += high_issues
        self.verification_results["mediumPriorityIssues"] += medium_issues
        
        return {
            "fileName": str(file_path.relative_to(self.repo_path)),
            "accuracyScore": accuracy_score,
            "issues": issues,
            "recommendations": self.generate_recommendations(issues)
        }

    def check_mtm_compliance(self, content, file_path):
        """Check MTM business logic compliance"""
        issues = []
        
        # Check TransactionType determination
        if "TransactionType" in content:
            for anti_pattern in self.mtm_validation_rules["TransactionType"]["antiPatterns"]:
                if re.search(anti_pattern, content, re.IGNORECASE):
                    issues.append({
                        "type": "MTM_TransactionType",
                        "severity": "Critical",
                        "description": f"Found incorrect TransactionType pattern: {anti_pattern}",
                        "file": file_path,
                        "rule": "TransactionType must be determined by USER INTENT, not Operation"
                    })
        
        # Check Operation number handling
        for invalid_format in self.mtm_validation_rules["OperationNumbers"]["invalidFormats"]:
            if re.search(f"Operation.*{invalid_format}[^\"']", content):
                issues.append({
                    "type": "MTM_OperationFormat",
                    "severity": "High",
                    "description": f"Operation number should be string format: \"{invalid_format}\" not {invalid_format}",
                    "file": file_path,
                    "rule": "Operation numbers are string workflow identifiers"
                })
        
        # Check MTM color scheme
        for color in self.mtm_validation_rules["ColorScheme"]["requiredColors"]:
            if color.lower().replace("#", "") in content.lower() and color not in content:
                issues.append({
                    "type": "MTM_ColorScheme",
                    "severity": "Medium",
                    "description": f"Color should use exact format: {color}",
                    "file": file_path,
                    "rule": "MTM color palette must use exact hex codes"
                })
        
        return issues

    def check_framework_compliance(self, content, file_path):
        """Check .NET 8 and Avalonia framework compliance"""
        issues = []
        
        # Check for deprecated .NET Framework references
        deprecated_patterns = [
            r"\.NET Framework",
            r"net461",
            r"net472",
            r"INotifyPropertyChanged.*(?!ReactiveUI)"
        ]
        
        for pattern in deprecated_patterns:
            if re.search(pattern, content, re.IGNORECASE):
                issues.append({
                    "type": "Framework_Deprecated",
                    "severity": "Critical",
                    "description": f"Found deprecated pattern: {pattern}",
                    "file": file_path,
                    "rule": "Must use .NET 8 and modern patterns"
                })
        
        # Check for ReactiveUI patterns
        if "ViewModel" in content and "ReactiveObject" not in content:
            if "INotifyPropertyChanged" in content:
                issues.append({
                    "type": "Framework_ReactiveUI",
                    "severity": "High",
                    "description": "ViewModels should inherit from ReactiveObject, not implement INotifyPropertyChanged",
                    "file": file_path,
                    "rule": "Use ReactiveUI patterns for ViewModels"
                })
        
        return issues

    def check_service_registration(self, content, file_path):
        """Check service registration accuracy"""
        issues = []
        
        # Check for correct AddMTMServices usage
        if "services.Add" in content:
            # Look for fictional service registration methods
            for anti_pattern in self.mtm_validation_rules["AddMTMServices"]["antiPatterns"]:
                if anti_pattern in content:
                    issues.append({
                        "type": "Service_Registration",
                        "severity": "Critical",
                        "description": f"Found fictional service registration method: {anti_pattern}",
                        "file": file_path,
                        "rule": "Use AddMTMServices(configuration) for service registration"
                    })
            
            # Check for correct service lifetime documentation
            if "AddSingleton<IDatabaseService>" in content:
                issues.append({
                    "type": "Service_Lifetime",
                    "severity": "High",
                    "description": "IDatabaseService should be Scoped, not Singleton",
                    "file": file_path,
                    "rule": "Database services should be Scoped for proper connection management"
                })
        
        return issues

    def check_cross_references(self, content, file_path):
        """Check for broken cross-references"""
        issues = []
        
        # Extract markdown links
        links = re.findall(r'\[.*?\]\((.*?)\)', content)
        
        for link in links:
            if not link.startswith(('http', 'mailto:')):
                # Resolve relative path
                if file_path.is_absolute():
                    base_path = file_path.parent
                else:
                    base_path = self.repo_path / file_path.parent
                
                target_path = base_path / link
                
                if not target_path.exists():
                    # Check if it's a reference to moved file
                    if link.endswith('.instruction.md'):
                        issues.append({
                            "type": "CrossReference_Moved",
                            "severity": "High",
                            "description": f"Reference to moved instruction file: {link}",
                            "file": str(file_path),
                            "rule": "Update references to new .github/ organized structure"
                        })
                    else:
                        issues.append({
                            "type": "CrossReference_Broken",
                            "severity": "Medium",
                            "description": f"Broken link: {link}",
                            "file": str(file_path),
                            "rule": "All internal links must be functional"
                        })
        
        return issues

    def generate_recommendations(self, issues):
        """Generate automated fix recommendations"""
        recommendations = []
        
        for issue in issues:
            if issue["type"] == "MTM_TransactionType":
                recommendations.append(
                    "Replace TransactionType switch statements with user intent-based logic"
                )
            elif issue["type"] == "Service_Registration":
                recommendations.append(
                    "Replace fictional service registration methods with AddMTMServices(configuration)"
                )
            elif issue["type"] == "CrossReference_Moved":
                recommendations.append(
                    "Update instruction file references to new .github/ organized structure"
                )
            elif issue["type"] == "Framework_ReactiveUI":
                recommendations.append(
                    "Update ViewModels to inherit from ReactiveObject and use RaiseAndSetIfChanged"
                )
        
        return list(set(recommendations))  # Remove duplicates

    def generate_report(self):
        """Generate comprehensive verification report"""
        total_files = len(self.verification_results["fileResults"])
        if total_files > 0:
            total_score = sum(r["accuracyScore"] for r in self.verification_results["fileResults"])
            self.verification_results["overallScore"] = total_score / total_files
        
        # Generate automated fix prompts
        self.verification_results["automatedFixPrompts"] = self.generate_fix_prompts()
        
        return self.verification_results

    def generate_fix_prompts(self):
        """Generate automated fix prompts for common issues"""
        fix_prompts = []
        
        # Service registration fix prompt
        if self.verification_results["criticalIssues"] > 0:
            fix_prompts.append({
                "type": "Service_Registration_Fix",
                "prompt": """Act as Quality Assurance Auditor Copilot. Fix service registration accuracy:

1. Replace all instances of AddCoreServices/AddBusinessServices/AddUtilityServices with AddMTMServices(configuration)
2. Update service lifetime documentation: IDatabaseService should be Scoped, not Singleton
3. Add missing services: ICacheService, IValidationService, IDbConnectionFactory
4. Verify all service names match actual implementations

Generate corrected service registration examples."""
            })
        
        # MTM business logic fix prompt
        if any("MTM_" in issue["type"] for file_result in self.verification_results["fileResults"] for issue in file_result["issues"]):
            fix_prompts.append({
                "type": "MTM_Business_Logic_Fix",
                "prompt": """Act as Quality Assurance Auditor Copilot. Fix MTM business logic compliance:

1. Update TransactionType determination to USER INTENT patterns
2. Correct Operation number formats to string workflow identifiers
3. Fix MTM color scheme references to exact hex codes
4. Remove any switch statements on Operation numbers

Generate corrected MTM business logic examples."""
            })
        
        return fix_prompts

def main():
    parser = argparse.ArgumentParser(description="MTM Documentation Accuracy Verification")
    parser.add_argument("repo_path", help="Path to repository root")
    parser.add_argument("--output", "-o", help="Output JSON file path", default="verification_report.json")
    parser.add_argument("--readme-only", action="store_true", help="Verify README files only")
    
    args = parser.parse_args()
    
    verifier = MTMDocumentationVerifier(args.repo_path)
    
    print("MTM Documentation Accuracy Verification")
    print("=" * 50)
    
    if args.readme_only:
        print("Verifying README files...")
        verifier.verify_readme_files()
    
    # Generate final report
    report = verifier.generate_report()
    
    # Save to JSON
    with open(args.output, 'w') as f:
        json.dump(report, f, indent=2)
    
    # Print summary
    print(f"\nVerification completed!")
    print(f"Overall Score: {report['overallScore']:.1f}%")
    print(f"Critical Issues: {report['criticalIssues']}")
    print(f"High Priority Issues: {report['highPriorityIssues']}")
    print(f"Medium Priority Issues: {report['mediumPriorityIssues']}")
    print(f"\nDetailed report saved to: {args.output}")

if __name__ == "__main__":
    main()