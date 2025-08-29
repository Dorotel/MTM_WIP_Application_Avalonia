<!-- Copilot: Reading custom-prompts.instruction.md — Custom Prompts and Copilot Personas -->

# Custom Copilot Prompts - Master Index

This file serves as the **master index** for all custom prompt types used in the MTM WIP Application Avalonia project. Each prompt type has a detailed implementation file in the `Documentation/Development/Custom_Prompts/` folder with complete usage examples, guidelines, and technical requirements.

> **See [personas.instruction.md](personas.instruction.md) for complete persona definitions and behavioral guidelines.**

---

## 🚀 **NEW: Comprehensive Hotkey System** ⭐

**Quick Access**: Use the **[MTM Hotkey Reference System](../../Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md)** for rapid prompt access!

### **Quick Shortcuts (50+ Prompts)**
```sh
# UI Generation
@ui:create @ui:viewmodel @ui:layout @ui:theme @ui:context

# Business Logic  
@biz:handler @biz:model @biz:db @biz:config @biz:crud

# Database Operations
@db:procedure @db:service @db:validation @db:error @db:schema

# Quality Assurance
@qa:verify @qa:refactor @qa:test @qa:pr @qa:infrastructure

# Core Systems
@sys:result @sys:di @sys:services @sys:nav @sys:state

# Compliance Fixes
@fix:01 @fix:02 @fix:03 @fix:04 @fix:05 @fix:06 @fix:07 @fix:08 @fix:09 @fix:10 @fix:11

# Error Handling & Documentation
@err:system @err:log @doc:api @doc:prompt @event:handler @issue:create

# NEW: Knowledge Assessment & Interactive Questionnaires
@qa:questions @qa:assessment @qa:quiz @qa:knowledge @qa:interactive @qa:questionnaire
```

**Example Usage**: `@ui:create inventory search component` instead of typing full file paths!

---

## 🧠 **NEW: Comprehensive Knowledge Assessment System** ⭐

<details>
<summary><strong>📚 Knowledge Assessment and Training Prompts</strong></summary>

### **🎯 Assessment Generation Capabilities**
When asked to create questions about any MTM WIP Application topic, use the following comprehensive assessment patterns:

#### **Question Types to Generate**
1. **Real-World Application Questions** - Complex scenarios requiring deep understanding
2. **Technical Implementation Questions** - Code patterns, architecture decisions, best practices
3. **Business Logic Questions** - MTM-specific workflows, transaction types, data patterns
4. **Integration Questions** - Service interactions, dependency injection, data flow
5. **Troubleshooting Questions** - Error handling, debugging, performance optimization
6. **Architecture Questions** - MVVM patterns, service organization, project structure

#### **HTML Assessment Template Structure**
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM WIP Application - [Topic] Assessment</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            min-height: 100vh;
            padding: 20px;
        }

        .assessment-container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 12px;
            box-shadow: 0 15px 35px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .assessment-header {
            background: linear-gradient(135deg, #6a0dad 0%, #4a0880 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }

        .assessment-header h1 {
            font-size: 2.5rem;
            margin-bottom: 10px;
            font-weight: 300;
        }

        .assessment-description {
            font-size: 1.1rem;
            opacity: 0.9;
            max-width: 600px;
            margin: 0 auto;
        }

        .question-bank {
            padding: 30px;
        }

        .question-container {
            background: #f8f9fa;
            border-radius: 8px;
            margin-bottom: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            overflow: hidden;
            transition: transform 0.2s ease;
        }

        .question-container:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }

        .question-header {
            background: #6a0dad;
            color: white;
            padding: 15px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 10px;
        }

        .question-number {
            font-weight: bold;
            font-size: 1.1rem;
        }

        .badge {
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: bold;
            text-transform: uppercase;
        }

        .difficulty-badge.beginner { background: #28a745; }
        .difficulty-badge.intermediate { background: #ffc107; color: #000; }
        .difficulty-badge.advanced { background: #fd7e14; }
        .difficulty-badge.expert { background: #dc3545; }

        .category-badge {
            background: #17a2b8;
            color: white;
        }

        .question-content {
            padding: 25px;
        }

        .question-title {
            color: #6a0dad;
            margin-bottom: 15px;
            font-size: 1.3rem;
        }

        .question-scenario {
            background: #e8f4fd;
            border-left: 4px solid #17a2b8;
            padding: 15px;
            margin-bottom: 20px;
            border-radius: 4px;
        }

        .question-scenario strong {
            color: #17a2b8;
        }

        .question-text {
            margin-bottom: 20px;
        }

        .question-text strong {
            color: #6a0dad;
        }

        .answer-options {
            display: grid;
            gap: 10px;
        }

        .option {
            background: white;
            border: 2px solid #e9ecef;
            border-radius: 6px;
            padding: 15px;
            cursor: pointer;
            transition: all 0.2s ease;
            position: relative;
        }

        .option:hover {
            border-color: #6a0dad;
            background: #f8f9ff;
        }

        .option.selected {
            border-color: #6a0dad;
            background: #f8f9ff;
            box-shadow: 0 2px 8px rgba(106, 13, 173, 0.2);
        }

        .option input[type="radio"] {
            margin-right: 10px;
            transform: scale(1.2);
        }

        .option label {
            cursor: pointer;
            display: block;
            font-weight: 500;
        }

        .performance-rating {
            position: absolute;
            top: 8px;
            right: 8px;
            font-size: 0.75rem;
            font-weight: bold;
            padding: 2px 6px;
            border-radius: 10px;
            color: white;
        }

        .performance-best { background: #28a745; }
        .performance-good { background: #17a2b8; }
        .performance-fair { background: #ffc107; color: #000; }
        .performance-poor { background: #fd7e14; }
        .performance-worst { background: #dc3545; }

        .explanation {
            background: #d4edda;
            border: 1px solid #c3e6cb;
            border-radius: 6px;
            padding: 20px;
            margin-top: 15px;
            display: none;
        }

        .explanation h4 {
            color: #155724;
            margin-bottom: 10px;
        }

        .explanation p {
            margin-bottom: 8px;
        }

        .explanation strong {
            color: #155724;
        }

        .performance-analysis {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 6px;
            padding: 15px;
            margin-top: 10px;
        }

        .performance-analysis h5 {
            color: #856404;
            margin-bottom: 8px;
            font-size: 0.9rem;
        }

        .performance-ranking {
            color: #856404;
            font-size: 0.85rem;
            line-height: 1.4;
        }

        .progress-indicator {
            background: #e9ecef;
            height: 8px;
            border-radius: 4px;
            margin: 20px 0;
            overflow: hidden;
        }

        .progress-bar {
            height: 100%;
            background: linear-gradient(90deg, #6a0dad, #4a0880);
            width: 0%;
            transition: width 0.3s ease;
        }

        .question-counter {
            text-align: center;
            color: #6c757d;
            margin-bottom: 20px;
        }

        .submit-section {
            text-align: center;
            padding: 30px;
            background: #f8f9fa;
        }

        .submit-btn {
            background: linear-gradient(135deg, #6a0dad 0%, #4a0880 100%);
            color: white;
            border: none;
            padding: 15px 40px;
            font-size: 1.1rem;
            border-radius: 6px;
            cursor: pointer;
            transition: transform 0.2s ease;
            margin: 0 10px;
        }

        .submit-btn:hover {
            transform: translateY(-2px);
        }

        .submit-btn.secondary {
            background: #6c757d;
        }

        .submit-btn.secondary:hover {
            background: #545b62;
        }

        .submit-btn.success {
            background: #28a745;
        }

        .submit-btn.success:hover {
            background: #218838;
        }

        .summary-section {
            display: none;
            margin-top: 30px;
            padding: 20px;
            background: #e8f5e8;
            border-radius: 8px;
            border: 2px solid #28a745;
        }

        .csv-output {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 4px;
            padding: 15px;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            white-space: pre-wrap;
            max-height: 200px;
            overflow-y: auto;
            margin: 15px 0;
        }

        .instructions {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 6px;
            padding: 15px;
            margin: 20px 0;
        }

        .mysql-version-notice {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 6px;
            padding: 15px;
            margin-bottom: 20px;
            color: #856404;
        }

        .mysql-version-notice strong {
            color: #856404;
        }

        @media (max-width: 768px) {
            .assessment-header h1 {
                font-size: 2rem;
            }
            
            .question-header {
                flex-direction: column;
                align-items: flex-start;
            }
            
            body {
                padding: 10px;
            }

            .performance-rating {
                position: relative;
                top: auto;
                right: auto;
                display: block;
                margin-top: 8px;
                width: fit-content;
            }

            .submit-btn {
                display: block;
                width: 100%;
                margin: 10px 0;
            }
        }
    </style>
</head>
<body>
    <div class="assessment-container">
        <header class="assessment-header">
            <h1>[Assessment Title]</h1>
            <p class="assessment-description">[Assessment description with scope and objectives]</p>
        </header>

        <div class="instructions">
            <strong>📋 Instructions:</strong> [Instructions for completing the assessment]
        </div>

        <div class="question-counter">
            <strong>Progress: <span id="progressText">0 of X</span> questions completed</strong>
        </div>

        <div class="progress-indicator">
            <div class="progress-bar" id="progressBar"></div>
        </div>
        
        <div class="question-bank">
            <!-- Questions will be generated here -->
        </div>

        <div class="submit-section">
            <button class="submit-btn secondary" onclick="resetForm()">🔄 Reset All Answers</button>
            <button class="submit-btn" onclick="validateAnswers()" id="validateBtn">✅ Validate Answers</button>
            <button class="submit-btn success" onclick="generateResults()" id="resultsBtn" style="display: none;">📊 Generate Results</button>
            <button class="submit-btn" onclick="showResults()" id="showResultsBtn" style="display: none;">📋 Show Results & Explanations</button>
        </div>

        <div class="summary-section" id="summarySection">
            <h3>📋 Assessment Results Summary</h3>
            <div id="summaryContent"></div>
            
            <h4>📄 Export Data:</h4>
            <div class="csv-output" id="csvOutput"></div>
            
            <div style="text-align: center; margin-top: 20px;">
                <button class="submit-btn" onclick="copyToClipboard()">📋 Copy to Clipboard</button>
                <button class="submit-btn secondary" onclick="downloadResults()">💾 Download Results</button>
            </div>
        </div>
    </div>

    <script>
        // Assessment JavaScript functionality
        let answers = {};
        let totalQuestions = 0;

        function selectOption(questionNum, optionLetter) {
            answers[questionNum] = optionLetter;
            
            // Update UI selection
            const questionSection = document.querySelector(`[data-question="${questionNum}"]`);
            const options = questionSection.querySelectorAll('.option');
            options.forEach(opt => opt.classList.remove('selected'));
            
            const selectedOption = document.querySelector(`#q${questionNum}${optionLetter.toLowerCase()}`).closest('.option');
            selectedOption.classList.add('selected');
            
            updateProgress();
        }

        function updateProgress() {
            const completedQuestions = Object.keys(answers).length;
            const progressPercent = (completedQuestions / totalQuestions) * 100;
            
            document.getElementById('progressBar').style.width = progressPercent + '%';
            document.getElementById('progressText').textContent = `${completedQuestions} of ${totalQuestions}`;
            
            if (completedQuestions === totalQuestions) {
                document.getElementById('validateBtn').style.display = 'inline-block';
            }
        }

        function validateAnswers() {
            const completedQuestions = Object.keys(answers).length;
            
            if (completedQuestions !== totalQuestions) {
                alert(`Please answer all ${totalQuestions} questions. You have answered ${completedQuestions}.`);
                return;
            }
            
            document.getElementById('resultsBtn').style.display = 'inline-block';
            document.getElementById('showResultsBtn').style.display = 'inline-block';
            document.getElementById('validateBtn').innerHTML = '✅ All Questions Completed!';
            document.getElementById('validateBtn').classList.add('success');
        }

        function generateResults() {
            // Generate CSV or other export format
            const timestamp = new Date().toISOString();
            let results = `Assessment Results Generated: ${timestamp}\n`;
            results += 'Question,Answer,Timestamp\n';
            
            for (let q = 1; q <= totalQuestions; q++) {
                const answer = answers[q];
                if (answer) {
                    results += `"Q${q}","${answer}","${timestamp}"\n`;
                }
            }
            
            document.getElementById('csvOutput').textContent = results;
            document.getElementById('summarySection').style.display = 'block';
        }

        function showResults() {
            const explanations = document.querySelectorAll('.explanation');
            explanations.forEach(explanation => {
                explanation.style.display = 'block';
            });
            
            document.querySelector('.assessment-container').scrollIntoView({
                behavior: 'smooth'
            });
        }

        function copyToClipboard() {
            const results = document.getElementById('csvOutput').textContent;
            navigator.clipboard.writeText(results).then(function() {
                alert('✅ Results copied to clipboard!');
            }, function(err) {
                console.error('Could not copy text: ', err);
                alert('❌ Failed to copy to clipboard. Please manually select and copy the results.');
            });
        }

        function downloadResults() {
            const results = document.getElementById('csvOutput').textContent;
            const blob = new Blob([results], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Assessment_Results_${new Date().toISOString().split('T')[0]}.csv`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        }

        function resetForm() {
            if (confirm('Are you sure you want to reset all answers?')) {
                answers = {};
                document.querySelectorAll('.option').forEach(opt => opt.classList.remove('selected'));
                document.querySelectorAll('input[type="radio"]').forEach(input => input.checked = false);
                document.getElementById('summarySection').style.display = 'none';
                document.getElementById('resultsBtn').style.display = 'none';
                document.getElementById('showResultsBtn').style.display = 'none';
                document.getElementById('validateBtn').style.display = 'none';
                document.getElementById('validateBtn').innerHTML = '✅ Validate Answers';
                document.getElementById('validateBtn').classList.remove('success');
                updateProgress();
            }
        }

        // Initialize progress tracking
        window.addEventListener('DOMContentLoaded', function() {
            totalQuestions = document.querySelectorAll('.question-container').length;
            updateProgress();
        });
    </script>
</body>
</html>
```

#### **Individual Question Template for Assessments**
```html
<div class="question-container" data-difficulty="[beginner|intermediate|advanced|expert]" data-question="[#]">
    <div class="question-header">
        <span class="question-number">Question [#]</span>
        <span class="badge difficulty-badge [difficulty-class]">[Difficulty Level]</span>
        <span class="badge category-badge">[Category]</span>
    </div>
    
    <div class="question-content">
        <h3 class="question-title">[Scenario-based question title]</h3>
        <div class="question-scenario">
            <p><strong>Scenario:</strong> [Real-world scenario context with specific MTM business requirements]</p>
            <p><strong>Current State:</strong> [Current implementation or situation]</p>
            <p><strong>Requirement:</strong> [Business requirement or technical objective]</p>
        </div>
        <div class="question-text">
            <p><strong>Question:</strong> [Specific technical question requiring analysis]</p>
        </div>
    </div>
    
    <div class="answer-options">
        <div class="option" onclick="selectOption([#], 'A')" data-correct="[true|false]">
            <span class="performance-rating performance-[best|good|fair|poor|worst]">[RATING]</span>
            <input type="radio" name="q[#]" id="q[#]a" value="a">
            <label for="q[#]a">[Option A with detailed technical explanation]</label>
        </div>
        <div class="option" onclick="selectOption([#], 'B')" data-correct="[true|false]">
            <span class="performance-rating performance-[best|good|fair|poor|worst]">[RATING]</span>
            <input type="radio" name="q[#]" id="q[#]b" value="b">
            <label for="q[#]b">[Option B with detailed technical explanation]</label>
        </div>
        <div class="option" onclick="selectOption([#], 'C')" data-correct="[true|false]">
            <span class="performance-rating performance-[best|good|fair|poor|worst]">[RATING]</span>
            <input type="radio" name="q[#]" id="q[#]c" value="c">
            <label for="q[#]c">[Option C with detailed technical explanation]</label>
        </div>
        <div class="option" onclick="selectOption([#], 'D')" data-correct="[true|false]">
            <span class="performance-rating performance-[best|good|fair|poor|worst]">[RATING]</span>
            <input type="radio" name="q[#]" id="q[#]d" value="d">
            <label for="q[#]d">[Option D with detailed technical explanation]</label>
        </div>
    </div>
    
    <div class="explanation">
        <h4>Explanation:</h4>
        <p><strong>Correct Answer:</strong> [Letter] - [Detailed explanation]</p>
        <p><strong>Why this matters:</strong> [Business/technical context]</p>
        <p><strong>Related concepts:</strong> [Cross-references to other topics]</p>
        
        <div class="performance-analysis">
            <h5>🚀 Performance Analysis:</h5>
            <div class="performance-ranking">
                <strong>1. BEST ([Letter]):</strong> [Performance reasoning]<br>
                <strong>2. GOOD ([Letter]):</strong> [Performance reasoning]<br>
                <strong>3. FAIR ([Letter]):</strong> [Performance reasoning]<br>
                <strong>4. POOR/WORST ([Letter]):</strong> [Performance reasoning]
            </div>
        </div>
    </div>
</div>
```

#### **Interactive Questionnaire Template**
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM WIP Application - [Topic] Configuration Questionnaire</title>
    <style>
        /* Use similar styles to assessment template but with questionnaire-specific modifications */
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            background: #f8f9fa;
            color: #333;
        }
        
        .container {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }
        
        h1 {
            color: #6a0dad;
            text-align: center;
            border-bottom: 3px solid #6a0dad;
            padding-bottom: 15px;
        }
        
        .question-section {
            margin: 30px 0;
            padding: 20px;
            border: 2px solid #e9ecef;
            border-radius: 8px;
            background: #fafbfc;
        }
        
        .question-title {
            font-size: 1.4em;
            font-weight: bold;
            color: #495057;
            margin-bottom: 15px;
        }
        
        .question-description {
            font-style: italic;
            color: #6c757d;
            margin-bottom: 20px;
        }
        
        .option {
            margin: 15px 0;
            padding: 15px;
            border: 1px solid #dee2e6;
            border-radius: 6px;
            background: white;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .option:hover {
            border-color: #6a0dad;
            background: #f8f7ff;
        }
        
        .option.selected {
            border-color: #6a0dad;
            background: #f8f7ff;
            box-shadow: 0 2px 8px rgba(106, 13, 173, 0.2);
        }
        
        .option-letter {
            font-weight: bold;
            color: #6a0dad;
            display: inline-block;
            width: 30px;
        }
        
        .option-title {
            font-weight: bold;
            color: #212529;
        }
        
        .option-description {
            color: #6c757d;
            margin-top: 5px;
        }
        
        .real-world-example {
            background: #e3f2fd;
            padding: 10px;
            border-left: 4px solid #2196f3;
            margin-top: 10px;
            font-style: italic;
        }
        
        .mtm-highlight {
            background: #f3e5f5;
            padding: 10px;
            border-left: 4px solid #6a0dad;
            margin-top: 10px;
        }
        
        /* Action buttons and CSV output styles similar to assessment */
    </style>
</head>
<body>
    <div class="container">
        <h1>[Questionnaire Title]</h1>
        
        <div class="instructions">
            <strong>📋 Instructions:</strong> [Questionnaire instructions]
        </div>

        <div class="question-counter">
            <strong>Progress: <span id="progressText">0 of X</span> questions completed</strong>
        </div>

        <div class="progress-indicator">
            <div class="progress-bar" id="progressBar"></div>
        </div>

        <form id="configurationQuestionnaire">
            <!-- Question sections go here -->
        </form>

        <div class="action-buttons">
            <button type="button" class="btn btn-secondary" onclick="resetForm()">🔄 Reset All Answers</button>
            <button type="button" class="btn btn-primary" onclick="validateAnswers()" id="validateBtn">✅ Validate Answers</button>
            <button type="button" class="btn btn-success" onclick="generateCSV()" id="csvBtn" style="display: none;">📊 Generate CSV Results</button>
        </div>

        <div class="summary-section" id="summarySection">
            <h3>📋 Configuration Summary</h3>
            <div id="summaryContent"></div>
            
            <h4>📄 CSV Export Data:</h4>
            <div class="csv-output" id="csvOutput"></div>
            
            <div style="text-align: center; margin-top: 20px;">
                <button type="button" class="btn btn-primary" onclick="copyCSV()">📋 Copy CSV to Clipboard</button>
                <button type="button" class="btn btn-secondary" onclick="downloadCSV()">💾 Download CSV File</button>
            </div>
        </div>
    </div>

    <script>
        // Interactive questionnaire JavaScript functionality
        let answers = {};
        let questionTitles = {
            // Question mapping goes here
        };

        let optionTitles = {
            // Option mapping goes here
        };

        function selectOption(questionNum, optionLetter) {
            answers[questionNum] = optionLetter;
            
            const radio = document.getElementById(`q${questionNum}${optionLetter.toLowerCase()}`);
            if (radio) {
                radio.checked = true;
            }
            
            const questionSection = document.querySelector(`[data-question="${questionNum}"]`);
            const options = questionSection.querySelectorAll('.option');
            options.forEach(opt => opt.classList.remove('selected'));
            
            event.currentTarget.classList.add('selected');
            
            updateProgress();
        }

        function updateProgress() {
            const totalQuestions = Object.keys(questionTitles).length;
            const completedQuestions = Object.keys(answers).length;
            const progressPercent = (completedQuestions / totalQuestions) * 100;
            
            document.getElementById('progressBar').style.width = progressPercent + '%';
            document.getElementById('progressText').textContent = `${completedQuestions} of ${totalQuestions}`;
            
            if (completedQuestions === totalQuestions) {
                document.getElementById('validateBtn').style.display = 'inline-block';
            }
        }

        function validateAnswers() {
            const totalQuestions = Object.keys(questionTitles).length;
            const completedQuestions = Object.keys(answers).length;
            
            if (completedQuestions !== totalQuestions) {
                alert(`Please answer all ${totalQuestions} questions. You have answered ${completedQuestions}.`);
                return;
            }
            
            document.getElementById('csvBtn').style.display = 'inline-block';
            document.getElementById('validateBtn').innerHTML = '✅ All Questions Completed!';
            document.getElementById('validateBtn').classList.add('btn-success');
            
            generateSummary();
        }

        function generateSummary() {
            let summaryHTML = '<div style="display: grid; gap: 15px;">';
            
            for (let q in answers) {
                const answer = answers[q];
                if (answer) {
                    summaryHTML += `
                        <div style="background: #f8f9fa; padding: 15px; border-radius: 6px; border-left: 4px solid #6a0dad;">
                            <strong>Q${q}: ${questionTitles[q]}</strong><br>
                            <span style="color: #6a0dad; font-weight: bold;">Answer ${answer}:</span> ${optionTitles[q][answer]}
                        </div>
                    `;
                }
            }
            summaryHTML += '</div>';
            
            document.getElementById('summaryContent').innerHTML = summaryHTML;
            document.getElementById('summarySection').style.display = 'block';
        }

        function generateCSV() {
            const timestamp = new Date().toISOString();
            let csvContent = 'Configuration Results\n';
            csvContent += `Generated: ${timestamp}\n`;
            csvContent += 'Question,Category,Answer,Choice\n';
            
            for (let q in answers) {
                const answer = answers[q];
                if (answer) {
                    csvContent += `"Q${q}","${questionTitles[q]}","${answer}","${optionTitles[q][answer]}"\n`;
                }
            }
            
            csvContent += '\nImplementation Notes:\n';
            csvContent += 'Use these answers for system configuration\n';
            csvContent += 'Follow MTM architectural patterns\n';
            csvContent += 'Integrate with existing services\n';
            
            document.getElementById('csvOutput').textContent = csvContent;
        }

        function copyCSV() {
            const csvText = document.getElementById('csvOutput').textContent;
            navigator.clipboard.writeText(csvText).then(function() {
                alert('✅ CSV data copied to clipboard!');
            }, function(err) {
                console.error('Could not copy text: ', err);
                alert('❌ Failed to copy to clipboard. Please manually select and copy the CSV data.');
            });
        }

        function downloadCSV() {
            const csvText = document.getElementById('csvOutput').textContent;
            const blob = new Blob([csvText], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Configuration_Results_${new Date().toISOString().split('T')[0]}.csv`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            alert('📁 CSV file downloaded!');
        }

        function resetForm() {
            if (confirm('Are you sure you want to reset all answers?')) {
                answers = {};
                document.getElementById('configurationQuestionnaire').reset();
                document.querySelectorAll('.option').forEach(opt => opt.classList.remove('selected'));
                document.getElementById('summarySection').style.display = 'none';
                document.getElementById('csvBtn').style.display = 'none';
                document.getElementById('validateBtn').style.display = 'none';
                updateProgress();
            }
        }

        // Initialize progress
        updateProgress();
    </script>
</body>
</html>
```

#### **Assessment Categories**
- **Architecture & Design Patterns** - MVVM, Service Layer, Dependency Injection
- **Database Operations** - Stored Procedures, Transaction Management, Error Handling
- **UI Development** - Avalonia Controls, Data Binding, Styling, User Experience
- **Business Logic** - MTM Workflows, Transaction Types, Validation Rules
- **Error Handling** - Exception Management, User Feedback, Logging
- **Performance & Security** - Optimization, Connection Management, Data Protection
- **Testing & Quality** - Unit Testing, Code Quality, Compliance Verification

#### **Difficulty Levels**
- **Beginner** - Basic concepts, simple implementations, fundamental patterns
- **Intermediate** - Complex scenarios, integration patterns, business rule implementation
- **Advanced** - Performance optimization, error handling, architectural decisions
- **Expert** - System design, advanced patterns, troubleshooting complex issues

#### **Interactive Features Required**
- **Progress Tracking** - Visual progress bar and question completion counters
- **Real-time Validation** - Immediate feedback on answer selection
- **Performance Ratings** - Color-coded performance indicators for each option
- **CSV Export** - Copy to clipboard and download functionality
- **Reset Capability** - Reset form with confirmation
- **Mobile Responsive** - Proper display on all device sizes
- **Accessibility** - WCAG AA compliance for all interactive elements

</details>

---

## 📁 **Folder Organization**

### **Custom Prompt Files Location**
All custom prompt implementation files are located in `Documentation/Development/Custom_Prompts/` with the naming format `CustomPrompt_{Action}_{Where}.md`.

### **HTML Documentation Integration**
Each custom prompt must maintain corresponding HTML documentation files in:
- `Documentation/HTML/PlainEnglish/custom-prompts.html` - Business-friendly explanations
- `Documentation/HTML/Technical/custom-prompts.html` - Developer-focused documentation

### **Assessment Files Integration** 🆕
Assessment and knowledge testing files should be organized in:
- `Documentation/HTML/Assessments/` - HTML questionnaire files using the comprehensive template above
- `Documentation/HTML/Assessments/styles/` - CSS styling for assessments (if external CSS needed)
- `Documentation/HTML/Assessments/scripts/` - JavaScript for interactive features (if external JS needed)

### **Interactive Questionnaire Integration** 🆕
Configuration and training questionnaires should be organized in:
- `Documentation/HTML/Questionnaires/` - Interactive questionnaire files
- `Documentation/HTML/Questionnaires/configs/` - Configuration-specific questionnaires
- `Documentation/HTML/Questionnaires/training/` - Training and onboarding questionnaires

### **Hotkey Integration** 🆕
- **[Comprehensive Hotkey Reference](../../Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md)** - All 50+ shortcuts with workflows
- **[Hotkey System Prompt](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_HotkeySystem.md)** - Template for hotkey enhancements

---

## 📋 **Quick Reference - Prompt Types**

### **🎨 UI Generation and Development**
- **[Create UI Element](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElement.md)** - For generating Avalonia controls or views based on mapping and instructions [`@ui:create`]
- **[Create UI Element from Markdown Instructions](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElementFromMarkdown.md)** - For generating Avalonia AXAML and Standard .NET ViewModels from parsed markdown files [`@ui:markdown`]
- **[Create Standard ViewModel](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_StandardViewModel.md)** - For generating ViewModels with standard .NET MVVM patterns, commands, and INotifyPropertyChanged [`@ui:viewmodel`]
- **[Create Modern Layout Pattern](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ModernLayoutPattern.md)** - For generating modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns [`@ui:layout`]
- **[Create Context Menu Integration](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ContextMenuIntegration.md)** - For adding context menus to components with management features following MTM patterns [`@ui:context`]
- **[Create Avalonia Theme Resources](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_AvaloniaThemeResources.md)** - For generating MTM-specific color schemes and theme resources using the purple brand palette [`@ui:theme`]

### **⚠️ Error Handling and Logging**
- **[Create Error System Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ErrorSystemPlaceholder.md)** - For scaffolding error handling classes with standard conventions [`@err:system`]
- **[Create Logging Info Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_LoggingInfoPlaceholder.md)** - For generating logging helpers and configuration according to project standards [`@err:log`]
- **[Create UI Element for Error Messages](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElementForErrorMessages.md)** - For generating error message UI components with MTM theme integration [`@ui:error`]

### **🔧 Business Logic and Data Access**
- **[Create Business Logic Handler](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_BusinessLogicHandler.md)** - For scaffolding business logic classes with proper naming and separation from UI [`@biz:handler`]
- **[Create Database Access Layer Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DatabaseAccessLayerPlaceholder.md)** - For generating database interaction classes or repositories [`@biz:db`]
- **[Create MTM Data Model](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_MTMDataModel.md)** - For generating models that follow MTM-specific data patterns [`@biz:model`]
- **[Create CRUD Operations](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CRUDOperations.md)** - For generating complete CRUD functionality [`@biz:crud`]

### **🗄️ Database Operations** 🆕
- **[Create Stored Procedure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_StoredProcedure.md)** - For creating new database stored procedures [`@db:procedure`]
- **[Update Stored Procedure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Update_StoredProcedure.md)** - For modifying existing stored procedures [`@db:update`]
- **[Database Error Handling](../../Documentation/Development/Custom_Prompts/CustomPrompt_Database_ErrorHandling.md)** - For database-specific error handling patterns [`@db:error`]
- **[Document Database Schema](../../Documentation/Development/Custom_Prompts/CustomPrompt_Document_DatabaseSchema.md)** - For creating database schema documentation [`@db:schema`]
- **[Create Database Service Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DatabaseServiceLayer.md)** - For implementing centralized database access [`@db:service`]
- **[Create Validation System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ValidationSystem.md)** - For implementing business rule validation [`@db:validation`]

### **📋 Testing and Configuration**
- **[Create Pull Request](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_Issue.md)** - Conducts comprehensive code quality reviews against all MTM WIP Application instruction guidelines [`@issue:create`]
- **[Create Unit Test Skeleton](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UnitTestSkeleton.md)** - For generating basic unit test classes with appropriate structure [`@qa:test`]
- **[Create Configuration File Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ConfigurationFilePlaceholder.md)** - For generating config/settings files with standard project structure [`@biz:config`]
- **[Create Unit Testing Infrastructure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UnitTestingInfrastructure.md)** - For setting up comprehensive testing framework with mocks and fixtures [`@qa:infrastructure`]

### **✅ Code Quality and Maintenance**
- **[Verify Code Compliance](../../Documentation/Development/Custom_Prompts/CustomPrompt_Verify_CodeCompliance.md)** - For conducting comprehensive quality assurance reviews against MTM instruction guidelines [`@qa:verify`]
- **[Refactor Code to Naming Convention](../../Documentation/Development/Custom_Prompts/CustomPrompt_Refactor_CodeToNamingConvention.md)** - For requesting code element renaming to fit project naming rules [`@qa:refactor`]
- **[Add Event Handler Stub](../../Documentation/Development/Custom_Prompts/CustomPrompt_Add_EventHandlerStub.md)** - For generating empty event handler methods with TODOs for specific controls [`@event:handler`]

### **📚 Documentation and Project Management**
- **[Document Public API/Class](../../Documentation/Development/Custom_Prompts/CustomPrompt_Document_PublicAPIClass.md)** - For generating XML documentation for public members [`@doc:api`]
- **[Create Customized Prompt](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CustomizedPrompt.md)** - For drafting new actionable custom prompts and assigning appropriate personas [`@doc:prompt`]
- **[Update Instruction File from Master](../../Documentation/Development/Custom_Prompts/CustomPrompt_Update_InstructionFileFromMaster.md)** - For synchronizing instruction files with relevant content from the main copilot-instructions.md [`@doc:update`]

---

## 🏗️ **Missing Core Systems Implementation Prompts**

*Based on analysis from needsrepair.instruction.md identifying missing core systems*

### **Phase 1: Foundation Systems**
- **[Implement Result Pattern System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ResultPatternSystem.md)** - For creating the Result<T> pattern infrastructure for consistent service responses [`@sys:result`]
- **[Create Data Models Foundation](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DataModelsFoundation.md)** - For generating the complete Models namespace with MTM-specific data entities [`@sys:foundation`]
- **[Setup Dependency Injection Container](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_DependencyInjectionContainer.md)** - For configuring Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs [`@sys:di`]
- **[Create Core Service Interfaces](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CoreServiceInterfaces.md)** - For generating all essential service interfaces following MTM patterns [`@sys:services`]

### **Phase 2: Service Layer Implementation**
- **[Implement Service Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ServiceLayer.md)** - For creating complete service implementations with MTM patterns and error handling [`@sys:layer`]
- **[Setup Application State Management](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_ApplicationStateManagement.md)** - For creating global application state service with proper encapsulation [`@sys:state`]
- **[Implement Configuration Service](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ConfigurationService.md)** - For creating service to read and manage appsettings.json configuration [`@sys:config`]

### **Phase 3: Infrastructure Systems**
- **[Create Navigation Service](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_NavigationService.md)** - For implementing proper MVVM navigation patterns with view-viewmodel mapping [`@sys:nav`]
- **[Implement Theme System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ThemeSystem.md)** - For creating MTM purple brand theme resources and DynamicResource patterns [`@sys:theme`]
- **[Setup Repository Pattern](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_RepositoryPattern.md)** - For implementing data access abstraction layer with repository interfaces [`@sys:repository`]

### **Phase 4: Quality Assurance Systems**
- **[Implement Structured Logging](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_StructuredLogging.md)** - For adding centralized logging throughout the application with proper levels [`@err:structured`]
- **[Create Caching Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CachingLayer.md)** - For implementing performance-oriented caching for ComboBox data and user preferences [`@sys:cache`]
- **[Setup Security Infrastructure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_SecurityInfrastructure.md)** - For implementing authentication, authorization, and secure connection management [`@sys:security`]

---

## 🔧 **Compliance Fix Prompts** 🆕

*Critical system fixes identified in quality analysis*

### **Database Foundation (COMPLETED ✅)**
- **[Fix 01: Empty Development Stored Procedures](../../Documentation/Development/Custom_Prompts/Compliance_Fix01_EmptyDevelopmentStoredProcedures.md)** - **COMPLETED** - Database foundation with 12 comprehensive procedures [`@fix:01`]

### **Foundation Fixes (READY 🚀)**
- **[Fix 02: Missing Standard Output Parameters](../../Documentation/Development/Custom_Prompts/Compliance_Fix02_MissingStandardOutputParameters.md)** - Template provided by Fix #1 [`@fix:02`]
- **[Fix 03: Inadequate Error Handling Stored Procedures](../../Documentation/Development/Custom_Prompts/Compliance_Fix03_InadequateErrorHandlingStoredProcedures.md)** - Pattern established by Fix #1 [`@fix:03`]
- **[Fix 04: Missing Service Layer Database Integration](../../Documentation/Development/Custom_Prompts/Compliance_Fix04_MissingServiceLayerDatabaseIntegration.md)** - Can use new procedures [`@fix:04`]
- **[Fix 05: Missing Data Models and DTOs](../../Documentation/Development/Custom_Prompts/Compliance_Fix05_MissingDataModelsAndDTOs.md)** - Can integrate with procedures [`@fix:05`]
- **[Fix 06: Missing Dependency Injection Container](../../Documentation/Development/Custom_Prompts/Compliance_Fix06_MissingDependencyInjectionContainer.md)** - Service layer foundation [`@fix:06`]

### **Infrastructure Fixes (ENABLED 🔧)**
- **[Fix 07: Missing Theme and Resource System](../../Documentation/Development/Custom_Prompts/Compliance_Fix07_MissingThemeAndResourceSystem.md)** - UI layer integration [`@fix:07`]
- **[Fix 08: Missing Input Validation Stored Procedures](../../Documentation/Development/Custom_Prompts/Compliance_Fix08_MissingInputValidationStoredProcedures.md)** - **RESOLVED** by Fix #1 [`@fix:08`]
- **[Fix 09: Inconsistent Transaction Management](../../Documentation/Development/Custom_Prompts/Compliance_Fix09_InconsistentTransactionManagement.md)** - **RESOLVED** by Fix #1 [`@fix:09`]
- **[Fix 10: Missing Navigation Service](../../Documentation/Development/Custom_Prompts/Compliance_Fix10_MissingNavigationService.md)** - UI infrastructure [`@fix:10`]
- **[Fix 11: Missing Configuration Service](../../Documentation/Development/Custom_Prompts/Compliance_Fix11_MissingConfigurationService.md)** - Service layer integration [`@fix:11`]

---

## 📘 **Rules for Managing Custom Prompts**

### **Rule 1: File Creation and Organization**
- **Whenever a new custom prompt is added to this master index, a corresponding detailed implementation file must be created in `Documentation/Development/Custom_Prompts/` folder using the naming format `CustomPrompt_{Action}_{Where}.md`.**
- **Each prompt file must include the complete template structure: Instructions, Persona, Prompt Template, Purpose, Usage Examples, Guidelines, Related Files, and Quality Checklist.**
- **All prompts must reference appropriate personas from [personas.instruction.md](personas.instruction.md).**

### **Rule 2: HTML Documentation Synchronization** ⭐ **NEW**
**Any time a custom prompt is created, removed, or edited, the corresponding HTML documentation files MUST be updated:**

#### **Required HTML Updates**
1. **Plain English Documentation**: Update `Documentation/HTML/PlainEnglish/custom-prompts.html`
   - Add business-friendly explanations of what the prompt accomplishes
   - Use manufacturing analogies and non-technical language
   - Focus on workflow benefits and process improvements
   - Include when and why to use each prompt from a business perspective

2. **Technical Documentation**: Update `Documentation/HTML/Technical/custom-prompts.html` 
   - Add detailed technical implementation guidance
   - Include code examples and integration patterns
   - Reference MTM-specific requirements and constraints
   - Provide troubleshooting and edge case handling

#### **HTML Update Process**
1. **Create/Edit Prompt**: Make changes to prompt files in `Documentation/Development/Custom_Prompts/`
2. **Update Master Index**: Update this file with new prompt references
3. **Update Plain English HTML**: Modify `Documentation/HTML/PlainEnglish/custom-prompts.html`
4. **Update Technical HTML**: Modify `Documentation/HTML/Technical/custom-prompts.html`
5. **Test Navigation**: Ensure all links work correctly between HTML files
6. **Validate Content**: Verify HTML content matches prompt file information

#### **HTML Content Standards**
- **Responsive Design**: Use all CSS files (modern-styles.css, mtm-theme.css, plain-english.css, responsive.css)
- **Cross-Reference Navigation**: Include navigation buttons between Plain English and Technical versions
- **MTM Branding**: Apply appropriate color schemes (Black/Gold for Plain English, Purple for Technical)
- **Accessibility**: Follow WCAG AA guidelines for all HTML content
- **Mobile Support**: Ensure proper display on mobile devices

### **Rule 3: Assessment and Questionnaire HTML Creation** 🆕 **NEW**
**When creating assessment or questionnaire HTML files, MUST use the comprehensive templates provided above:**

#### **Assessment HTML Requirements**
1. **Use Assessment Template**: Apply the complete HTML assessment template structure
2. **Include Interactive Features**: Progress tracking, performance ratings, CSV export, reset capability
3. **Performance Analysis**: Each option must include performance ratings (BEST, GOOD, FAIR, POOR, WORST)
4. **Detailed Explanations**: Comprehensive explanations with business context and cross-references
5. **Real-world Scenarios**: Complex scenarios requiring deep MTM understanding
6. **Mobile Responsive**: Proper display on all device sizes with mobile-specific adjustments

#### **Interactive Questionnaire Requirements**
1. **Use Questionnaire Template**: Apply the interactive questionnaire template structure
2. **Configuration Focus**: Design for gathering configuration preferences and requirements
3. **Real-world Examples**: Include practical examples for each option
4. **MTM Highlights**: Show specific MTM business impact for each choice
5. **CSV Export**: Generate structured CSV output for implementation guidance
6. **Progress Tracking**: Visual progress indicators and completion validation

#### **Required Interactive Features**
- **Copy to Clipboard**: JavaScript functionality to copy results
- **Download CSV**: Automatic CSV file download capability
- **Reset Form**: Confirmation dialog before clearing all answers
- **Progress Tracking**: Real-time progress bar and completion counters
- **Answer Validation**: Ensure all questions answered before proceeding
- **Mobile Support**: Touch-friendly interface for mobile devices

### **Rule 4: Hotkey Integration** 🆕 **NEW**
**Any time a custom prompt is created, removed, or edited, the hotkey system MUST be updated:**

#### **Required Hotkey Updates**
1. **Update Hotkey Reference**: Modify `Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md`
   - Add new shortcuts following category conventions (@ui:, @biz:, @db:, @qa:, @sys:, @err:, @doc:, @fix:, @event:, @issue:)
   - Include usage examples and workflow integration
   - Update quick reference tables and statistics

2. **Update Master Index**: Add hotkey shortcuts in brackets after each prompt link (e.g., [`@ui:create`])

3. **Test Shortcuts**: Verify hotkey patterns are memorable and consistent with existing conventions

### **Rule 5: Quality and Compliance**
- All custom prompt files must follow MTM coding conventions and instruction guidelines
- Each prompt must include proper error handling patterns
- Reference organized instruction files appropriately
- Maintain consistency with existing documentation structure
- HTML files must validate against W3C standards
- All interactive features must be tested across browsers and devices

---

## 🎯 **Usage Guidelines**

For detailed usage guidelines, MTM-specific workflow patterns, persona behavioral guidelines, and integration with development workflow, see the comprehensive guide in [Documentation/Development/Custom_Prompts/README.md](../../Documentation/Development/Custom_Prompts/README.md).

### **Quick Usage Steps**
1. **Use Hotkeys**: Try the shortcut first (e.g., `@ui:create component_name`)
2. **Navigate to Specific Prompt**: Click on the prompt link to access the detailed implementation file
3. **Review Persona Requirements**: Check the assigned persona and behavioral guidelines
4. **Copy Template**: Use the provided prompt template as-is or customize for your specific needs
5. **Follow Guidelines**: Adhere to the technical requirements and MTM-specific patterns
6. **Check Examples**: Review usage examples for context and proper implementation
7. **Update HTML Documentation**: If creating/editing prompts, update corresponding HTML files
8. **Create Interactive Content**: Use assessment and questionnaire templates for training materials

### **Hotkey Workflow Examples** 🆕
```sh
# Quick UI Development
@ui:create → @ui:viewmodel → @ui:theme → @qa:verify

# Database Operations
@db:schema → @db:procedure → @db:service → @fix:04

# Compliance Resolution
@qa:verify → @fix:XX → @qa:refactor → @qa:test → @qa:pr

# Complete Feature Development
@ui:create → @biz:handler → @db:procedure → @qa:test → @qa:verify

# Assessment and Training Creation
@qa:assessment → @qa:interactive → @qa:questionnaire → @doc:update
```

### **HTML Creation Workflow Examples** 🆕
```sh
# Assessment Creation
1. Use @qa:assessment hotkey
2. Apply HTML assessment template
3. Generate questions with performance analysis
4. Include interactive features (progress, CSV export, reset)
5. Test across browsers and devices
6. Validate W3C compliance

# Interactive Questionnaire Creation  
1. Use @qa:questionnaire hotkey
2. Apply questionnaire template
3. Include real-world examples and MTM highlights
4. Add configuration-focused questions
5. Implement CSV export for implementation guidance
6. Test mobile responsiveness
```

---

*This master index provides centralized access to all 50+ custom prompts for the MTM WIP Application Avalonia project, now with comprehensive hotkey integration, interactive HTML templates, and assessment creation capabilities for maximum development efficiency. Each prompt is designed to maintain consistency with MTM standards and accelerate development while ensuring quality and compliance.*
