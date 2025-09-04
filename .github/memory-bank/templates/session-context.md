# Development Session Context Template

**Session Date**: [YYYY-MM-DD]  
**Session Duration**: [Start Time] - [End Time] ([Duration])  
**Developer(s)**: [Names/Usernames]  
**Session Type**: [Feature Development | Bug Fix | Refactoring | Investigation | Documentation]

## Session Overview
<!-- Brief summary of what this development session aimed to accomplish -->

## Feature/Issue Context
**Related Issues**: #  
**Related Epic/Feature**: #  
**Priority Level**: [Critical | High | Medium | Low]  

### Business Context
<!-- Why is this work important? What business value does it provide? -->

### Technical Context
<!-- What technical challenges or considerations are involved? -->

## Architecture and Design Decisions

### Current System State
<!-- Describe the current state of the system before changes -->

### Proposed Changes
<!-- What changes are being made and why -->

### Design Decisions Made
1. **Decision**: [Brief description]
   - **Rationale**: [Why this decision was made]
   - **Alternatives Considered**: [Other options that were evaluated]
   - **Trade-offs**: [What was gained/lost with this decision]

### MTM Pattern Compliance
- [ ] **MVVM Community Toolkit**: Changes follow `[ObservableProperty]` and `[RelayCommand]` patterns
- [ ] **Service Architecture**: Integration with established service layer patterns
- [ ] **Database Access**: Uses stored procedures only via `Helper_Database_StoredProcedure`
- [ ] **Avalonia AXAML**: Follows proper AXAML syntax with `x:Name` requirements
- [ ] **Error Handling**: Uses centralized `Services.ErrorHandling.HandleErrorAsync()`
- [ ] **Manufacturing Domain**: Aligns with inventory management business context

## Implementation Context

### Components Being Modified
**ViewModels**:
- 

**Services**:
- 

**Views (AXAML)**:
- 

**Models**:
- 

**Database**:
- 

### Key Implementation Challenges
1. **Challenge**: [Description]
   - **Approach**: [How you're addressing it]
   - **Status**: [In Progress | Resolved | Blocked]

### Code Examples and Patterns
```csharp
// Key code examples demonstrating patterns used
// This helps GitHub Copilot understand the context better

```

## Development Environment Context

### Technology Stack State
- **.NET Version**: 8.0
- **Avalonia Version**: 11.3.4
- **MVVM Community Toolkit**: 8.3.2
- **MySQL Version**: 9.4.0
- **IDE**: [Visual Studio | Visual Studio Code | JetBrains Rider]
- **GitHub Copilot**: [Enabled | Disabled]

### Local Environment Setup
<!-- Any specific environment setup or configuration needed -->

### Dependencies and Tools
<!-- List any new dependencies or tools introduced -->

## Testing Context

### Testing Strategy
- [ ] **Unit Tests**: [Planned | In Progress | Complete]
- [ ] **Integration Tests**: [Planned | In Progress | Complete]
- [ ] **Manual Testing**: [Planned | In Progress | Complete]
- [ ] **Performance Testing**: [Planned | In Progress | Complete | Not Applicable]

### Test Scenarios
1. **Scenario**: [Description]
   - **Expected Result**: [What should happen]
   - **Actual Result**: [What actually happened]
   - **Status**: [Pass | Fail | Not Tested]

## Issues and Blockers Encountered

### Technical Issues
1. **Issue**: [Description]
   - **Impact**: [How it affects the work]
   - **Resolution**: [How it was resolved, or current status]
   - **Prevention**: [How to avoid this in the future]

### Knowledge Gaps
1. **Gap**: [What wasn't clear or understood]
   - **Research Done**: [What investigation was done]
   - **Resolution**: [What was learned]
   - **Documentation**: [Where this knowledge should be captured]

## Progress and Outcomes

### Completed Work
- [ ] [Task 1]
- [ ] [Task 2]
- [ ] [Task 3]

### In Progress Work
- [ ] [Task in progress with current status]

### Deferred/Blocked Work
- [ ] [Task that was deferred with reason]

### Unexpected Discoveries
<!-- Any surprising findings or learnings -->

## Next Session Planning

### Immediate Next Steps
1. [Next action item]
2. [Another action item]

### Dependencies for Next Session
- [ ] [What needs to be ready before next session]

### Questions for Team/Stakeholders
1. [Question that needs answering]
2. [Another question]

## Knowledge Capture

### New Patterns Discovered
<!-- Any new code patterns or approaches worth documenting -->

### Lessons Learned
1. **Lesson**: [What was learned]
   - **Impact**: [How this will affect future development]
   - **Action**: [What should be done with this knowledge]

### GitHub Copilot Effectiveness
**Helpful Suggestions**: [Examples of where Copilot provided good suggestions]  
**Context Gaps**: [Areas where Copilot needed better context]  
**Pattern Recognition**: [How well Copilot understood MTM patterns]

### Documentation Updates Needed
- [ ] **GitHub Instructions**: [What needs updating in .github/instructions/]
- [ ] **Architecture Docs**: [What architectural documentation needs updates]
- [ ] **Code Comments**: [Areas where code documentation should be improved]

## References and Links

### Related Documentation
- [Link to relevant GitHub instructions]
- [Link to architecture documents]
- [Link to PRD or feature specifications]

### External Resources Used
- [Stack Overflow posts, documentation, tutorials, etc.]

### Code References
- **Commit SHA**: [If applicable]
- **Branch**: [Feature branch name]
- **Related PRs**: #

---

**Template Version**: 1.0  
**Last Updated**: September 4, 2025  
**Part of**: MTM Memory Bank System - Context Preservation