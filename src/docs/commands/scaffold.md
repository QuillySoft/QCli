# 🏗️ scaffold Command

Create complete project structure from templates.

## Overview

The `scaffold` command creates an entire new project with the proper directory structure, configuration files, and initial setup based on the selected template. It's perfect for starting new projects with QCLI's architectural patterns.

## Syntax

```powershell
qcli scaffold <ProjectName> [options]
```

## Arguments

| Argument | Description | Required |
|----------|-------------|----------|
| `ProjectName` | Name of the project to create | Yes |

## Options

| Option | Description | Default | Example |
|--------|-------------|---------|---------|
| `-t, --template <template>` | Project template to use | `clean-architecture` | `qcli scaffold MyApp -t minimal` |
| `-p, --path <path>` | Output directory path | Current directory | `qcli scaffold MyApp -p C:\Projects` |

## Available Templates

### clean-architecture (Default)
Complete Clean Architecture setup with all layers and patterns.

**Generated Structure:**
```
MyProject/
├── src/
│   ├── Core/
│   │   ├── Domain/                 # Domain entities, value objects
│   │   └── Application/            # Commands, queries, DTOs
│   ├── Infrastructure/
│   │   └── Persistence/            # Entity configurations, repositories
│   └── Apps/
│       └── Api/                    # Controllers, presentation layer
├── tests/
│   ├── Application.Tests/          # Unit tests
│   └── Integration.Tests/          # Integration tests
├── docs/
│   └── architecture.md            # Architecture documentation
├── qcli.json                      # QCLI configuration
├── .gitignore                     # Git ignore file
└── README.md                      # Project documentation
```

**Features:**
- ✅ Complete Clean Architecture layers
- ✅ CQRS with MediatR patterns
- ✅ Domain-driven design structure
- ✅ Comprehensive testing setup
- ✅ Authorization framework
- ✅ Entity Framework configurations
- ✅ API controllers structure

### minimal
Simplified project structure for rapid prototyping.

**Generated Structure:**
```
MyProject/
├── src/
│   ├── Domain/                     # Core business logic
│   ├── Application/                # Services and DTOs
│   └── WebApi/                     # API controllers
├── qcli.json                      # QCLI configuration
├── .gitignore                     # Git ignore file
└── README.md                      # Project documentation
```

**Features:**
- ✅ Basic layer separation
- ✅ Essential CRUD patterns
- ✅ Minimal dependencies
- ❌ No domain events
- ❌ No advanced patterns
- ✅ Quick startup

### ddd (Domain-Driven Design)
Rich domain model with advanced DDD patterns.

**Generated Structure:**
```
MyProject/
├── src/
│   ├── Domain/
│   │   ├── Entities/              # Rich domain entities
│   │   ├── ValueObjects/          # Value objects
│   │   ├── Events/                # Domain events
│   │   └── Services/              # Domain services
│   ├── Application/
│   │   ├── Commands/              # Command handlers
│   │   ├── Queries/               # Query handlers
│   │   └── Events/                # Event handlers
│   └── Infrastructure/            # External concerns
├── tests/
│   ├── Domain.Tests/              # Domain unit tests
│   ├── Application.Tests/         # Application tests
│   └── Integration.Tests/         # Integration tests
├── docs/
│   └── domain-model.md           # Domain model documentation
├── qcli.json
├── .gitignore
└── README.md
```

**Features:**
- ✅ Rich domain entities
- ✅ Domain events and handlers
- ✅ Value objects support
- ✅ Domain services
- ✅ Event sourcing ready
- ✅ Advanced DDD patterns

## Usage Examples

### Basic Scaffolding
```powershell
# Create project with default template
qcli scaffold ECommerceApi
```

**Output:**
```
🏗️ Scaffolding new project: ECommerceApi

Creating project structure...
✅ Created directory: ECommerceApi/src/Core/Domain
✅ Created directory: ECommerceApi/src/Core/Application
✅ Created directory: ECommerceApi/src/Infrastructure/Persistence
✅ Created directory: ECommerceApi/src/Apps/Api
✅ Created directory: ECommerceApi/tests/Application.Tests
✅ Created directory: ECommerceApi/tests/Integration.Tests
✅ Created directory: ECommerceApi/docs

Generating project files...
✅ Created: README.md
✅ Created: .gitignore
✅ Created: qcli.json
✅ Created: docs/architecture.md

🎉 Project 'ECommerceApi' scaffolded successfully!

📋 Next steps:
1. cd ECommerceApi
2. qcli config show
3. qcli add Product --all
4. Review generated README.md for details
```

### Template-Specific Scaffolding

#### Minimal Template
```powershell
qcli scaffold QuickAPI -t minimal
```

#### DDD Template
```powershell
qcli scaffold ComplexDomain -t ddd
```

#### Custom Output Path
```powershell
qcli scaffold MyProject -p "C:\Development\Projects"
```

### Advanced Examples

#### E-commerce Platform
```powershell
# Create comprehensive e-commerce platform
qcli scaffold ECommercePlatform -t clean-architecture -p "C:\Projects"

# Navigate and configure
cd "C:\Projects\ECommercePlatform"

# Configure for e-commerce
qcli config set project.namespace "ECommerce.Core"
qcli config set codeGeneration.defaultEntityType "FullyAudited"
qcli config set codeGeneration.generateEvents true

# Generate core entities
qcli add Product --all
qcli add Category --all
qcli add Order --all
qcli add Customer --all
```

#### Microservice
```powershell
# Create microservice
qcli scaffold OrderService -t minimal

cd OrderService

# Configure for microservice
qcli config set project.namespace "Company.OrderService"
qcli config set paths.domainPath "src/Domain"
qcli config set paths.applicationPath "src/Application"

# Generate service entities
qcli add Order --all
qcli add OrderItem --all
```

#### Domain-Rich Application
```powershell
# Create DDD application
qcli scaffold FinancialSystem -t ddd

cd FinancialSystem

# Configure for complex domain
qcli config set project.namespace "Financial.Core"
qcli config set codeGeneration.generateEvents true

# Generate domain entities
qcli add Account --all
qcli add Transaction --all
qcli add Portfolio --all
```

## Generated Files

### Configuration File (qcli.json)
```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json",
  "version": "1.0",
  "project": {
    "name": "ECommerceApi",
    "namespace": "ECommerceApi",
    "description": "A clean-architecture architecture project",
    "author": "",
    "version": "1.0.0"
  },
  "paths": {
    "domainPath": "src/Core/Domain",
    "applicationPath": "src/Core/Application",
    "persistencePath": "src/Infrastructure/Persistence",
    "apiPath": "src/Apps/Api",
    "testsPath": "tests"
  },
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": false,
    "generateMappingProfiles": false,
    "generatePermissions": true,
    "generateTests": true
  },
  "templates": {
    "defaultTemplate": "clean-architecture"
  },
  "projectType": "CleanArchitecture"
}
```

### README.md Template
The generated README includes:
- Project overview and architecture
- Getting started instructions
- Development workflow
- Build and test commands
- Deployment guidelines
- Contributing guidelines

### Architecture Documentation
For Clean Architecture and DDD templates, generates:
- `docs/architecture.md` - Architecture overview
- Layer descriptions and responsibilities
- Design patterns and principles
- Integration guidelines

### Git Configuration
- `.gitignore` with appropriate exclusions for .NET projects
- Ready for version control initialization

## Project Types and Use Cases

### Enterprise Applications
```powershell
# Large-scale business applications
qcli scaffold EnterpriseERP -t clean-architecture

# Features:
# - Complete separation of concerns
# - Comprehensive testing
# - Authorization framework
# - Audit trails
# - Event handling
```

### Rapid Prototypes
```powershell
# Quick proof-of-concept applications
qcli scaffold MVPPrototype -t minimal

# Features:
# - Fast setup
# - Essential patterns only
# - Minimal dependencies
# - Quick iteration
```

### Domain-Complex Systems
```powershell
# Systems with rich business logic
qcli scaffold TradingPlatform -t ddd

# Features:
# - Rich domain models
# - Domain events
# - Value objects
# - Business rule enforcement
```

## Post-Scaffolding Workflow

### 1. Initial Setup
```powershell
# Navigate to project
cd MyProject

# Verify configuration
qcli config show

# Check project health
qcli doctor
```

### 2. Generate Core Entities
```powershell
# Generate business entities
qcli add Customer --all --entity-type FullyAudited
qcli add Product --all --entity-type Audited
qcli add Order --all --entity-type FullyAudited

# Verify generation
qcli list entities
```

### 3. Build and Test
```powershell
# Build project
dotnet build

# Run tests
dotnet test

# Check for issues
qcli doctor
```

### 4. Initialize Git
```powershell
# Initialize repository
git init
git add .
git commit -m "Initial project scaffold"
```

## Customization

### Template Customization
You can customize generated templates by:
1. Modifying template files in QCLI source
2. Creating custom template overrides
3. Using configuration to control generation

### Project Structure Adaptation
After scaffolding, you can:
1. Modify paths via `qcli config set`
2. Add additional layers or projects
3. Integrate with existing solutions

## Integration with Existing Solutions

### Add to Existing Solution
```powershell
# Scaffold in subdirectory
qcli scaffold MyNewService -p "ExistingSolution\Services"

# Update solution file to include new project
dotnet sln add "Services\MyNewService\src\Apps\Api\MyNewService.Api.csproj"
```

### Microservice Architecture
```powershell
# Create multiple services
qcli scaffold UserService -t minimal -p "Services"
qcli scaffold OrderService -t minimal -p "Services"
qcli scaffold ProductService -t minimal -p "Services"

# Each service gets its own configuration and structure
```

## Common Patterns

### Monolithic Application
```powershell
# Single large application
qcli scaffold MonolithApp -t clean-architecture

# Generate all business entities in one project
qcli add Customer --all
qcli add Product --all
qcli add Order --all
# ... etc
```

### Modular Monolith
```powershell
# Create modules as separate projects
qcli scaffold SalesModule -t clean-architecture -p "Modules"
qcli scaffold InventoryModule -t clean-architecture -p "Modules"
qcli scaffold CustomerModule -t clean-architecture -p "Modules"
```

### Event-Driven Architecture
```powershell
# Create with event support
qcli scaffold EventDrivenApp -t ddd

# Configure for events
qcli config set codeGeneration.generateEvents true

# Generate entities with events
qcli add Order --all  # Will include domain events
```

## Troubleshooting

### Directory Already Exists
```
❌ Error: Directory 'MyProject' already exists
```
**Solution**: Choose different name or remove existing directory

### Permission Issues
```
❌ Error: Access denied creating directory
```
**Solution**: Run with administrator privileges or choose different path

### Template Not Found
```
❌ Error: Template 'invalid-template' not found
```
**Solution**: Use `qcli list templates` to see available templates

### Invalid Project Name
```
❌ Error: Invalid project name 'Invalid-Name!'
```
**Solution**: Use valid C# identifier (letters, numbers, underscore only)

## Best Practices

1. **Project Naming**: Use PascalCase names (e.g., `ECommerceApi`)
2. **Template Selection**: Choose template based on project complexity
3. **Path Organization**: Use clear directory structure for multiple projects
4. **Configuration Review**: Always review generated configuration
5. **Version Control**: Initialize Git repository after scaffolding
6. **Documentation**: Update generated README with project-specific information

---

**Related Documentation:**
- [init Command](init.md) - Initialize QCLI in existing projects
- [Templates Guide](../configuration/templates.md) - Template system details
- [Project Setup](../project-setup.md) - Complete project setup guide
