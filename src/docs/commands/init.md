# 🏗️ init Command

Initialize QCLI configuration in a new or existing project.

## Overview

The `init` command sets up QCLI in your project by creating a configuration file that defines paths, generation settings, and project metadata. This is typically the first command you run when setting up QCLI in a project.

## Syntax

```powershell
qcli init [options]
```

## Options

| Option | Description | Example |
|--------|-------------|---------|
| `--force` | Overwrite existing configuration | `qcli init --force` |
| `-t, --template <template>` | Use specific template | `qcli init --template minimal` |
| `--interactive` | Interactive configuration setup | `qcli init --interactive` |

## Available Templates

| Template | Description | Use Case |
|----------|-------------|----------|
| `clean-architecture` | Complete Clean Architecture setup | Full enterprise applications |
| `minimal` | Minimal setup with basic layers | Simple projects, prototypes |
| `ddd` | Domain-Driven Design template | Complex business domains |

## Usage Examples

### Quick Setup (Recommended)
```powershell
# Initialize with default Clean Architecture template
qcli init
```

**Generated Configuration:**
```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json",
  "version": "1.0",
  "project": {
    "name": "MyProject",
    "namespace": "MyProject"
  },
  "paths": {
    "domainPath": "src/Core/Domain",
    "applicationPath": "src/Core/Application",
    "persistencePath": "src/Infrastructure/Persistence",
    "apiPath": "src/Apps/Api"
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

### Interactive Setup
```powershell
qcli init --interactive
```

**Interactive Prompts:**
```
📝 Let's set up your project configuration...

? Project name: ECommercePlatform
? Root namespace: ECommerce.Core
? Project description (optional): Modern e-commerce platform
? Author name (optional): John Doe

📁 Configure project paths:
? Domain path: [src/Core/Domain] src/Domain
? Application path: [src/Core/Application] src/Application  
? Persistence path: [src/Infrastructure/Persistence] src/Data
? WebApi path: [src/Apps/Api] src/Api

⚙️ Code generation settings:
? Generate tests? (Y/n) Y
? Generate permissions? (Y/n) Y
? Generate domain events? (y/N) Y

✅ QCLI initialized successfully!
Configuration saved to: C:\Projects\ECommerce\qcli.json
```

### Template-Specific Setup

#### Minimal Template
```powershell
qcli init --template minimal
```

**Minimal Configuration:**
```json
{
  "project": {
    "name": "SimpleApp",
    "namespace": "SimpleApp"
  },
  "paths": {
    "domainPath": "src/Domain",
    "applicationPath": "src/Application",
    "apiPath": "src/WebApi"
  },
  "codeGeneration": {
    "generateEvents": false,
    "generateMappingProfiles": false,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

#### DDD Template
```powershell
qcli init --template ddd
```

**DDD Configuration:**
```json
{
  "project": {
    "name": "DomainDrivenApp",
    "namespace": "DomainDrivenApp"
  },
  "codeGeneration": {
    "generateEvents": true,
    "generatePermissions": true,
    "generateTests": true
  },
  "projectType": "DomainDrivenDesign"
}
```

### Force Overwrite
```powershell
# Overwrite existing configuration
qcli init --force

# Prompt appears:
# QCLI is already initialized. Do you want to reinitialize? (y/N)
```

## Configuration Templates

### Clean Architecture (Default)
Perfect for enterprise applications with full separation of concerns:

```
Project Structure:
src/
├── Core/
│   ├── Domain/              # Domain entities, value objects
│   └── Application/         # Commands, queries, DTOs
├── Infrastructure/
│   └── Persistence/         # Entity configurations, repositories
└── Apps/
    └── Api/                 # Controllers, presentation layer
tests/
├── Application/             # Unit tests
└── Integration/             # Integration tests
```

**Features:**
- ✅ Full Clean Architecture layers
- ✅ CQRS with MediatR
- ✅ Comprehensive testing
- ✅ Authorization system
- ✅ Entity Framework configurations

### Minimal Template
Simplified structure for smaller projects:

```
Project Structure:
src/
├── Domain/                  # Core business logic
├── Application/             # Services and DTOs
└── WebApi/                  # API controllers
```

**Features:**
- ✅ Basic layer separation
- ✅ Essential CRUD operations
- ❌ No domain events
- ❌ No mapping profiles
- ✅ Basic testing

### DDD Template
Rich domain model with advanced patterns:

```
Project Structure:
src/
├── Domain/
│   ├── Entities/           # Rich domain entities
│   ├── ValueObjects/       # Value objects
│   ├── Events/             # Domain events
│   └── Services/           # Domain services
├── Application/
│   ├── Commands/           # Command handlers
│   ├── Queries/            # Query handlers
│   └── Events/             # Event handlers
└── Infrastructure/         # External concerns
```

**Features:**
- ✅ Rich domain entities
- ✅ Domain events
- ✅ Value objects
- ✅ Domain services
- ✅ Event sourcing ready

## Configuration File Location

QCLI creates the configuration file as:
- Primary: `qcli.json` 
- Fallback: `quillysoft-cli.json`

The file is created in the current working directory.

## Validation

QCLI validates the configuration during initialization:

### ✅ Valid Configuration
```
✅ QCLI initialized successfully!
Configuration saved to: /path/to/qcli.json
```

### ❌ Invalid Configuration
```
❌ Configuration validation failed:
- Invalid namespace: Must be valid C# identifier
- Path not found: src/NonExistent/Path
- Invalid entity type: MustBeEntity/Audited/FullyAudited

Fix configuration file or run 'qcli config validate'
```

## Post-Initialization

After running `init`, you can:

1. **View Configuration**
   ```powershell
   qcli config show
   ```

2. **Modify Settings**
   ```powershell
   qcli config set codeGeneration.generateEvents true
   ```

3. **Generate First Entity**
   ```powershell
   qcli add Product --all
   ```

## Project-Specific Examples

### E-commerce Platform
```powershell
qcli init --interactive

# Set during interactive setup:
# Project: ECommercePlatform
# Namespace: ECommerce.Core
# Entity Type: FullyAudited (for order tracking)
# Events: true (for order processing)
```

### CRM System
```powershell
qcli init --template ddd

# Configure after init:
qcli config set project.namespace "CRM.Sales"
qcli config set codeGeneration.defaultEntityType "FullyAudited"
```

### Microservice
```powershell
qcli init --template minimal

# Configure for microservice:
qcli config set paths.domainPath "src/Domain"
qcli config set paths.applicationPath "src/Application"
qcli config set project.namespace "Company.ProductService"
```

### Legacy Integration
```powershell
qcli init

# Adapt to existing structure:
qcli config set paths.domainPath "Business/Models"
qcli config set paths.applicationPath "Business/Services"
qcli config set paths.persistencePath "Data/Mappings"
qcli config set project.namespace "LegacyApp.Business"
```

## Advanced Configuration

### Custom Project Structure
```powershell
qcli init --interactive

# During setup, customize paths:
# Domain: src/Core/Business
# Application: src/Core/Services  
# Persistence: src/Data/EntityFramework
# API: src/WebService
```

### Team Standards
```powershell
qcli init

# Apply team conventions:
qcli config set project.namespace "Company.Product.Core"
qcli config set codeGeneration.defaultEntityType "FullyAudited"
qcli config set codeGeneration.generateEvents true
```

## Troubleshooting

### Directory Already Initialized
```
QCLI is already initialized. Do you want to reinitialize? (y/N)
```
- Choose `y` to overwrite existing configuration
- Use `--force` to skip the prompt

### Permission Errors
```powershell
# Ensure write permissions to current directory
# Run as administrator if needed (Windows)
sudo qcli init  # Linux/macOS
```

### Invalid Template
```
❌ Template 'invalid-template' not found
Available templates: clean-architecture, minimal, ddd
```

### Path Creation Issues
```
❌ Failed to validate paths: Directory creation failed
```
- Ensure sufficient disk space
- Check directory permissions
- Verify path format

## Best Practices

1. **Start Simple**: Use default settings initially
2. **Team Alignment**: Use same template across team
3. **Version Control**: Commit `qcli.json` to repository
4. **Environment Specific**: Consider separate configs for dev/prod
5. **Namespace Convention**: Use consistent namespace patterns

## Next Steps

After initialization:

1. **Verify Setup**
   ```powershell
   qcli config show
   ```

2. **Generate First Entity**
   ```powershell
   qcli add Product --all
   ```

3. **Customize Configuration**
   ```powershell
   qcli config set codeGeneration.generateEvents true
   ```

---

**Related Documentation:**
- [Configuration Guide](../configuration/configuration.md) - Complete configuration reference
- [add Command](add.md) - Generate CRUD operations
- [Project Setup](../project-setup.md) - Complete project setup guide
