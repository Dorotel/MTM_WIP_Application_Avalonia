# MTM Task Classification System

## Overview
The MTM Task Classification System provides a comprehensive framework for categorizing, prioritizing, and organizing all development work within the MTM WIP Application project. This system aligns with the established Phase 1 architectural patterns and manufacturing domain context.

## Classification Dimensions

### 1. Component-Based Classification

#### **UI and Presentation Layer**
- **Label**: `component:ui`  
- **Description**: User interface components, AXAML files, visual design, themes
- **Examples**: Views, UserControls, Styles, Themes, Layout components
- **Pattern Alignment**: Avalonia AXAML syntax, MTM design system, responsive design

#### **ViewModels and Business Logic** 
- **Label**: `component:viewmodel`
- **Description**: MVVM Community Toolkit ViewModels, business logic, data binding
- **Examples**: ViewModels, Commands, Properties, Validation logic  
- **Pattern Alignment**: `[ObservableProperty]`, `[RelayCommand]`, BaseViewModel inheritance

#### **Services and Infrastructure**
- **Label**: `component:service`
- **Description**: Service layer components, dependency injection, cross-cutting concerns
- **Examples**: Business services, Infrastructure services, Utilities, Extensions
- **Pattern Alignment**: Interface-based services, Constructor injection, Result patterns

#### **Models and Data Structures**
- **Label**: `component:model`  
- **Description**: Domain models, DTOs, data structures, validation models
- **Examples**: Business entities, Request/Response models, Configuration classes
- **Pattern Alignment**: Manufacturing domain entities, Clean architecture principles

#### **Database and Data Access**
- **Label**: `component:database`
- **Description**: Database operations, stored procedures, data access patterns
- **Examples**: Stored procedures, Database schema, Migration scripts
- **Pattern Alignment**: Stored procedures only, `Helper_Database_StoredProcedure` usage

#### **Behaviors and Converters**
- **Label**: `component:behavior`
- **Description**: Avalonia behaviors, value converters, attached properties
- **Examples**: Validation behaviors, UI behaviors, Data converters
- **Pattern Alignment**: Avalonia behavior patterns, MVVM binding support

#### **Configuration and Settings**
- **Label**: `component:configuration`
- **Description**: Application configuration, settings management, environment setup
- **Examples**: appsettings, Configuration services, Environment variables
- **Pattern Alignment**: Microsoft.Extensions.Configuration patterns

### 2. Complexity Classification

#### **Simple Tasks**
- **Label**: `complexity:simple` 
- **Criteria**: Straightforward implementation, well-understood requirements, minimal risk
- **Estimation**: 1-8 hours
- **Examples**: Bug fixes, small UI adjustments, configuration changes
- **Approval**: Single reviewer required

#### **Moderate Tasks**
- **Label**: `complexity:moderate`
- **Criteria**: Some technical challenges, clear requirements, manageable scope
- **Estimation**: 1-3 days  
- **Examples**: New UI screens, Service enhancements, Model extensions
- **Approval**: Technical review + domain expert review

#### **Complex Tasks**
- **Label**: `complexity:complex`
- **Criteria**: Multiple components affected, architectural decisions, significant scope
- **Estimation**: 1-2 weeks
- **Examples**: Major features, Service integrations, Database schema changes
- **Approval**: Architecture review + Technical lead approval

#### **Epic Tasks**
- **Label**: `complexity:epic`
- **Criteria**: Multiple features, cross-cutting changes, major architectural impact  
- **Estimation**: 3+ weeks
- **Examples**: System redesigns, Major workflow changes, Technology upgrades
- **Approval**: Full architecture board review + Stakeholder approval

### 3. Domain Classification

#### **Manufacturing Operations**
- **Label**: `domain:manufacturing`
- **Description**: Core manufacturing processes, inventory operations, production workflows
- **Business Context**: Part tracking, Operation sequences (90→100→110→120), Transaction management
- **Examples**: Inventory transactions, Operation workflows, Part management

#### **Inventory Management**
- **Label**: `domain:inventory`
- **Description**: Inventory tracking, stock management, quantity operations
- **Business Context**: IN/OUT/TRANSFER transactions, Stock levels, Location tracking  
- **Examples**: Quick actions, Stock adjustments, Inventory reports

#### **Master Data Management**
- **Label**: `domain:masterdata`
- **Description**: Reference data management, configuration data, lookup tables
- **Business Context**: Part IDs, Locations, Operations, User management
- **Examples**: Part catalogs, Location setup, Operation definitions

#### **System Administration**
- **Label**: `domain:administration` 
- **Description**: System configuration, user management, security, maintenance
- **Business Context**: User permissions, System settings, Audit trails
- **Examples**: User management, System configuration, Backup procedures

#### **Quality and Compliance**
- **Label**: `domain:quality`
- **Description**: Quality assurance, compliance tracking, audit functionality
- **Business Context**: Audit trails, Compliance reporting, Quality metrics
- **Examples**: Audit logs, Quality reports, Compliance dashboards

#### **Integration and Automation**
- **Label**: `domain:integration`
- **Description**: System integrations, automation workflows, external connections
- **Business Context**: ERP integration, Automated workflows, Data synchronization
- **Examples**: API integrations, Scheduled jobs, Data import/export

### 4. Priority Classification

#### **Critical Priority**
- **Label**: `priority:critical`
- **Criteria**: Production issues, security vulnerabilities, data integrity problems
- **SLA**: 4 hours response, 24 hours resolution
- **Examples**: Application crashes, Data corruption, Security breaches
- **Escalation**: Immediate notification to technical lead and product owner

#### **High Priority**  
- **Label**: `priority:high`
- **Criteria**: Important features, significant user impact, business-critical functionality
- **SLA**: 24 hours response, 1 week resolution
- **Examples**: Key features, Performance issues, Major usability problems
- **Escalation**: Notification to technical lead within 24 hours

#### **Medium Priority**
- **Label**: `priority:medium`
- **Criteria**: Standard features, moderate user impact, planned enhancements
- **SLA**: 3 days response, 2 weeks resolution
- **Examples**: Feature enhancements, Minor bugs, User experience improvements
- **Escalation**: Standard workflow, no special escalation

#### **Low Priority**
- **Label**: `priority:low`
- **Criteria**: Nice-to-have features, minimal user impact, technical debt
- **SLA**: 1 week response, flexible resolution
- **Examples**: Code cleanup, Minor optimizations, Future considerations
- **Escalation**: Backlog management, addressed during maintenance cycles

### 5. Work Type Classification

#### **Feature Development**
- **Label**: `type:feature`
- **Description**: New functionality implementation, capability additions
- **Process**: Feature specification → Design → Implementation → Testing → Deployment
- **Templates**: Feature request template, Feature implementation PR template

#### **Bug Fixes**
- **Label**: `type:bug` 
- **Description**: Defect resolution, error correction, unexpected behavior fixes
- **Process**: Bug report → Investigation → Fix → Testing → Deployment
- **Templates**: Bug report template, Standard PR template

#### **Enhancement**
- **Label**: `type:enhancement`
- **Description**: Improvements to existing functionality, optimization, user experience
- **Process**: Enhancement request → Analysis → Implementation → Validation → Deployment  
- **Templates**: Enhancement template, Standard PR template

#### **Technical Debt**
- **Label**: `type:technical-debt`
- **Description**: Code quality improvements, refactoring, architecture improvements
- **Process**: Technical debt identification → Planning → Implementation → Validation
- **Templates**: Technical enabler template, Standard PR template

#### **Documentation**
- **Label**: `type:documentation`
- **Description**: Documentation creation, updates, knowledge management
- **Process**: Documentation request → Research → Writing → Review → Publication
- **Templates**: Documentation improvement template, Documentation PR template

#### **Testing**
- **Label**: `type:testing`
- **Description**: Test creation, test automation, quality assurance activities
- **Process**: Testing scope → Test design → Implementation → Execution → Reporting
- **Templates**: Technical enabler template, Standard PR template

## Label Management

### GitHub Labels Configuration

#### **Automated Labels** (Applied by GitHub Actions)
```yaml
# Component labels - applied based on file changes
component:ui           # AXAML, Views, Themes, Styles
component:viewmodel    # ViewModels folder changes  
component:service      # Services folder changes
component:model        # Models folder changes
component:database     # Database scripts, stored procedures
component:behavior     # Behaviors, Converters
component:configuration # Config files, appsettings

# Complexity labels - applied based on PR size and content analysis
complexity:simple      # < 50 lines changed, single component
complexity:moderate    # 50-200 lines, 2-3 components  
complexity:complex     # 200-500 lines, multiple components
complexity:epic        # > 500 lines, significant changes

# Domain labels - applied based on content analysis
domain:manufacturing   # Manufacturing operations context
domain:inventory       # Inventory management context  
domain:masterdata      # Master data management
domain:administration  # System administration
domain:quality         # Quality and compliance
domain:integration     # Integration and automation

# Priority labels - applied based on issue template selection
priority:critical      # Critical issue template
priority:high          # High priority selection in templates
priority:medium        # Medium priority selection
priority:low           # Low priority selection

# Type labels - applied based on title prefixes and templates
type:feature           # Feature templates, feat: prefixes
type:bug               # Bug templates, fix: prefixes
type:enhancement       # Enhancement templates, enhance: prefixes
type:technical-debt    # Technical enabler templates, refactor: prefixes
type:documentation     # Documentation templates, docs: prefixes
type:testing           # Testing related changes, test: prefixes
```

#### **Manual Labels** (Applied by project managers/leads)
```yaml
# Status labels
status:blocked         # Task is blocked by dependencies
status:in-progress     # Currently being worked on
status:ready-for-review # Ready for code review
status:ready-to-merge   # Approved and ready for merge
status:deployed        # Successfully deployed

# Special handling labels  
needs-discussion       # Requires team discussion
needs-architecture     # Requires architecture review
needs-design          # Requires UI/UX design
needs-testing         # Requires additional testing
breaking-change       # Contains breaking changes
```

### Label Application Workflow

#### **Automatic Application** (via GitHub Actions)
1. **Issue Creation**: Labels applied based on template selection and content analysis
2. **PR Creation**: Labels applied based on file changes, size, and title analysis  
3. **Content Updates**: Labels updated when issue/PR content changes
4. **Status Updates**: Workflow status labels applied automatically

#### **Manual Review Process**
1. **Weekly Label Review**: Project manager reviews label accuracy
2. **Priority Adjustment**: Priorities adjusted based on business needs
3. **Complexity Validation**: Technical lead validates complexity assignments
4. **Domain Classification**: Domain expert validates domain classifications

### Classification Reports

#### **Weekly Classification Report**
```yaml
Component Distribution:
  UI: 25% (15 issues)
  ViewModel: 20% (12 issues)  
  Service: 18% (11 issues)
  Database: 15% (9 issues)
  Model: 12% (7 issues)
  Other: 10% (6 issues)

Complexity Distribution:
  Simple: 40% (24 issues)
  Moderate: 35% (21 issues)
  Complex: 20% (12 issues)  
  Epic: 5% (3 issues)

Priority Distribution:
  Critical: 5% (3 issues)
  High: 25% (15 issues)
  Medium: 50% (30 issues)
  Low: 20% (12 issues)

Domain Distribution:
  Manufacturing: 35% (21 issues)
  Inventory: 30% (18 issues)
  Master Data: 15% (9 issues)
  Administration: 10% (6 issues)
  Quality: 5% (3 issues)
  Integration: 5% (3 issues)
```

## Integration with MTM Patterns

### MVVM Community Toolkit Alignment
- **Component Classification**: Clear separation between View, ViewModel, and Service work
- **Pattern Compliance**: Labels track adherence to `[ObservableProperty]` and `[RelayCommand]` patterns
- **Dependency Injection**: Service classification aligns with DI container organization

### Database Pattern Alignment  
- **Database Classification**: Specific labels for stored procedure and schema work
- **Pattern Enforcement**: Classification validates stored procedure only approach
- **Integration Testing**: Database labels trigger appropriate testing workflows

### Manufacturing Domain Alignment
- **Business Context**: Domain labels align with manufacturing operations
- **Workflow Integration**: Classification supports operation sequence workflows (90→100→110→120)  
- **Transaction Context**: Inventory labels distinguish transaction types (IN/OUT/TRANSFER)

### GitHub Copilot Enhancement
- **Context Enrichment**: Classifications provide rich context for AI assistance
- **Pattern Recognition**: Labels help Copilot understand code organization patterns
- **Knowledge Transfer**: Classification system preserves domain knowledge for AI learning

## Quality Metrics

### Classification Accuracy Metrics
- **Auto-labeling Accuracy**: Percentage of automatically applied labels that are correct
- **Manual Override Rate**: Frequency of manual label corrections  
- **Classification Consistency**: Consistency of labeling across similar work items
- **Domain Expert Validation**: Accuracy rate when validated by domain experts

### Process Effectiveness Metrics
- **Time to Classification**: Average time from issue creation to complete classification
- **Classification Coverage**: Percentage of issues with complete classification
- **Label Utilization**: Usage distribution across all available labels
- **Workflow Efficiency**: Impact of classification on development workflow speed

---

**System Status**: ✅ Classification Framework Complete  
**Implementation**: GitHub Actions automation + Manual oversight  
**Integration**: Phase 1 patterns + Phase 2 automation infrastructure  
**Maintained By**: MTM Development Team