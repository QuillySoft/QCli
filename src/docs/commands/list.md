# 📋 list Command

List available templates, entities, or operations in your QCLI project.

## Overview

The `list` command provides information about what's available in your QCLI project. It can display available project templates, discovered entities in your codebase, and supported CRUD operations with their options.

## Syntax

```powershell
qcli list <type>
```

## Arguments

| Argument | Description | Required |
|----------|-------------|----------|
| `type` | What to list (`templates`, `entities`, `operations`) | Yes |

## List Types

### templates
Display available project templates for initialization and scaffolding.

### entities  
Show discovered entities in your project with their current operations.

### operations
Display available CRUD operations and their command-line options.

## Usage Examples

### List Available Templates
```powershell
qcli list templates
```

**Example Output:**
```
📋 Available Templates:

┌───────────────────┬──────────────────────────────────────────────┬─────────────────┐
│ Template          │ Description                                  │ Status          │
├───────────────────┼──────────────────────────────────────────────┼─────────────────┤
│ clean-architecture│ Complete Clean Architecture with all layers │ ✅ Default      │
│ minimal           │ Minimal setup with basic layers             │ 🚧 Coming Soon  │
│ ddd               │ Domain-Driven Design with rich domain model │ 🚧 Coming Soon  │
│ microservice      │ Microservice template with API Gateway      │ 🚧 Coming Soon  │
│ blazor            │ Blazor Server/WASM with Clean Architecture   │ 🚧 Coming Soon  │
└───────────────────┴──────────────────────────────────────────────┴─────────────────┘

Usage: qcli init -t <template> or qcli scaffold <project> -t <template>
```

### List Project Entities
```powershell
qcli list entities
```

**Example Output:**
```
📋 Discovered Entities in Project:

┌──────────────┬─────────────────────────────────────┬─────────────────────────────┐
│ Entity       │ Path                                │ Operations                  │
├──────────────┼─────────────────────────────────────┼─────────────────────────────┤
│ Product      │ src/Core/Domain/Entities            │ ✅ CRUD ✅ Tests ✅ Perms   │
│ Category     │ src/Core/Domain/Entities            │ ✅ CRUD ✅ Tests ✅ Perms   │
│ Order        │ src/Core/Domain/Entities            │ ✅ CR-D ❌ Tests ✅ Perms   │
│ Customer     │ src/Core/Domain/Entities            │ ✅ C--D ❌ Tests ❌ Perms   │
│ OrderItem    │ src/Core/Domain/Entities            │ ❌ ---- ❌ Tests ❌ Perms   │
└──────────────┴─────────────────────────────────────┴─────────────────────────────┘

Legend:
✅ = Implemented  ❌ = Missing  
CRUD = Create/Read/Update/Delete  Tests = Unit Tests  Perms = Permissions

💡 Use 'qcli add <Entity> --missing' to generate missing operations
```

### List Available Operations
```powershell
qcli list operations
```

**Example Output:**
```
📋 Available CRUD Operations:

┌───────────┬─────────────────────────────────────┬─────────────────────────────────────┐
│ Flag      │ Description                         │ Example                             │
├───────────┼─────────────────────────────────────┼─────────────────────────────────────┤
│ --all     │ Generate all CRUD operations        │ qcli add Order --all                │
│ --create  │ Generate Create operation           │ qcli add Order --create             │
│ --read    │ Generate Read operations (queries)  │ qcli add Order --read               │
│ --update  │ Generate Update operation           │ qcli add Order --update             │
│ --delete  │ Generate Delete operation           │ qcli add Order --delete             │
└───────────┴─────────────────────────────────────┴─────────────────────────────────────┘

Additional Options:
  --entity-type <type>    Entity type (Audited, FullyAudited)
  --no-tests             Skip generating tests
  --no-permissions       Skip generating permissions
  --template <template>  Use specific template
  --dry-run             Preview changes without creating files
```

## Template Information

### Available Templates

#### clean-architecture (Default)
- **Status**: ✅ Available
- **Description**: Complete Clean Architecture with all layers
- **Use Case**: Enterprise applications with full separation of concerns
- **Structure**: Domain/Application/Infrastructure/Presentation layers
- **Features**: CQRS, DDD, comprehensive testing, authorization

#### minimal (Coming Soon)
- **Status**: 🚧 Development
- **Description**: Minimal setup with basic layers
- **Use Case**: Simple projects, prototypes, learning
- **Structure**: Domain/Application/WebApi
- **Features**: Basic CRUD, essential patterns only

#### ddd (Coming Soon)
- **Status**: 🚧 Development  
- **Description**: Domain-Driven Design with rich domain model
- **Use Case**: Complex business domains
- **Structure**: Rich domain entities, value objects, domain events
- **Features**: Advanced DDD patterns, event sourcing ready

#### microservice (Coming Soon)
- **Status**: 🚧 Development
- **Description**: Microservice template with API Gateway
- **Use Case**: Distributed systems, microservice architecture
- **Structure**: Independent service with API gateway integration
- **Features**: Service discovery, health checks, containerization

#### blazor (Coming Soon)
- **Status**: 🚧 Development
- **Description**: Blazor Server/WASM with Clean Architecture backend
- **Use Case**: Full-stack .NET applications
- **Structure**: Blazor frontend + Clean Architecture backend
- **Features**: Real-time UI, component-based development

## Entity Discovery

### Detection Logic
QCLI discovers entities by analyzing:
- **File patterns**: `*Entity.cs`, `*Aggregate.cs` files
- **Base class inheritance**: Classes inheriting from Entity, AuditedEntity, etc.
- **Directory structure**: Files in configured domain paths
- **Namespace patterns**: Classes in domain namespaces

### Entity Analysis
For each discovered entity, QCLI checks:
- **CRUD Operations**: Create, Read, Update, Delete command/query files
- **Test Coverage**: Unit and integration test files
- **Permissions**: Authorization permission classes
- **Configurations**: Entity Framework configurations
- **Controllers**: API controller implementations

### Operation Status Indicators

| Symbol | Meaning |
|--------|---------|
| ✅ | Fully implemented |
| ❌ | Not implemented |
| ⚠️ | Partially implemented |
| 🚧 | Under development |

### CRUD Status Legend
```
✅ CRUD = All operations (Create, Read, Update, Delete)
✅ CR-D = Create, Read, Delete (missing Update)
✅ C--D = Create, Delete only (missing Read, Update)
❌ ---- = No operations implemented
```

## Operations Reference

### Core Operations

#### --all
Generates complete CRUD functionality:
- Domain entity
- Create command + validator
- Update command + validator  
- Delete command
- Get by ID query
- Get list query with pagination
- DTOs for create/update
- API controller
- Entity configuration
- Permissions
- Unit tests
- Integration tests

#### --create
Generates create functionality:
- Create command
- Command validator
- Create endpoint in controller
- Create permissions
- Unit tests for create

#### --read
Generates read functionality:
- Get by ID query
- Get list query with pagination
- Read DTOs
- Read endpoints in controller
- Read permissions
- Unit tests for queries

#### --update
Generates update functionality:
- Update command
- Command validator
- Update endpoint in controller
- Update permissions
- Unit tests for update

#### --delete
Generates delete functionality:
- Delete command
- Delete endpoint in controller
- Delete permissions
- Unit tests for delete

### Additional Options

#### Entity Types
```powershell
# Basic entity (ID only)
qcli add Category --all --entity-type Entity

# Audited entity (with audit fields)
qcli add Product --all --entity-type Audited

# Fully audited (with soft delete)
qcli add Order --all --entity-type FullyAudited
```

#### Generation Control
```powershell
# Skip test generation
qcli add SimpleEntity --all --no-tests

# Skip permission generation
qcli add InternalEntity --all --no-permissions

# Use specific template
qcli add CustomEntity --all --template minimal

# Preview without creating files
qcli add TestEntity --all --dry-run
```

## Entity Status Examples

### Fully Implemented Entity
```
Product  │ src/Core/Domain/Entities  │ ✅ CRUD ✅ Tests ✅ Perms
```
- All CRUD operations exist
- Complete test coverage
- Full permission structure
- Ready for production use

### Partially Implemented Entity
```
Order    │ src/Core/Domain/Entities  │ ✅ CR-D ❌ Tests ✅ Perms
```
- Missing Update operation
- No test coverage
- Has basic permissions
- Needs completion

### New Entity
```
OrderItem│ src/Core/Domain/Entities  │ ❌ ---- ❌ Tests ❌ Perms
```
- Domain entity exists but no operations
- No CRUD functionality generated
- Needs full implementation

## Filtering and Search

### Filter by Status (Future Feature)
```powershell
# List only entities missing tests
qcli list entities --missing-tests

# List entities with partial implementation
qcli list entities --incomplete

# List fully implemented entities
qcli list entities --complete
```

### Search Entities (Future Feature)
```powershell
# Search for specific entities
qcli list entities --filter "Product*"

# List entities by type
qcli list entities --type FullyAudited
```

## Integration with Other Commands

### Generate Missing Operations
```powershell
# List entities to see what's missing
qcli list entities

# Generate missing operations for specific entity
qcli add Order --update  # Add missing Update operation

# Complete partial implementation
qcli add Customer --read --update  # Add missing operations
```

### Template Usage
```powershell
# List available templates
qcli list templates

# Use template for initialization
qcli init --template clean-architecture

# Use template for scaffolding
qcli scaffold MyProject --template minimal
```

### Operation Planning
```powershell
# See available operations
qcli list operations

# Plan entity generation
qcli add NewEntity --dry-run --all

# Generate with specific operations
qcli add NewEntity --create --read
```

## Scripting and Automation

### PowerShell Integration
```powershell
# Get entity list as JSON (future feature)
$entities = qcli list entities --format json | ConvertFrom-Json

# Check for incomplete entities
$incomplete = $entities | Where-Object { $_.Operations -notmatch "✅ CRUD" }

# Generate missing operations
foreach ($entity in $incomplete) {
    Write-Host "Completing $($entity.Name)..."
    qcli add $entity.Name --all
}
```

### CI/CD Integration
```powershell
# Verify all entities are complete
qcli list entities --format json | ConvertFrom-Json | ForEach-Object {
    if ($_.Operations -notmatch "✅ CRUD ✅ Tests ✅ Perms") {
        Write-Error "Entity $($_.Name) is incomplete"
        exit 1
    }
}
```

## Best Practices

1. **Regular Reviews**: Run `qcli list entities` regularly to track project completeness
2. **Incremental Development**: Use entity list to identify what needs implementation
3. **Template Planning**: Review available templates before starting new projects
4. **Operation Selection**: Use `qcli list operations` to understand available options
5. **Status Monitoring**: Monitor entity implementation status in team workflows

---

**Related Documentation:**
- [add Command](add.md) - Generate CRUD operations
- [init Command](init.md) - Initialize with templates
- [Templates Guide](../configuration/templates.md) - Template system details
