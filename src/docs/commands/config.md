# ⚙️ config Command

Manage QCLI configuration settings for your project.

## Overview

The `config` command provides comprehensive configuration management for QCLI projects. It allows you to initialize, view, modify, and validate configuration settings that control code generation behavior.

## Syntax

```powershell
qcli config <subcommand> [options]
```

## Subcommands

| Subcommand | Description | Example |
|------------|-------------|---------|
| `init` | Initialize configuration file | `qcli config init` |
| `show` | Display current configuration | `qcli config show` |
| `set` | Set a configuration value | `qcli config set paths.domainPath src/Domain` |
| `get` | Get a configuration value | `qcli config get paths.domainPath` |
| `sample` | Generate sample configuration | `qcli config sample` |

## Options

| Option | Description | Example |
|--------|-------------|---------|
| `-p, --path <path>` | Configuration file path | `qcli config show --path custom.json` |
| `-k, --key <key>` | Configuration key | `qcli config get --key paths.domainPath` |
| `--value <value>` | Configuration value | `qcli config set --key generateTests --value true` |
| `-g, --global` | Use global configuration | `qcli config show --global` |

## Configuration File Structure

QCLI uses a `qcli.json` (or `quillysoft-cli.json`) configuration file:

```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json",
  "version": "1.0",
  "project": {
    "name": "MyProject",
    "namespace": "MyProject",
    "description": "A Clean Architecture project",
    "author": "Your Name",
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
    "defaultTemplate": "clean-architecture",
    "customTemplatesPath": "templates",
    "enableCustomTemplates": false,
    "templateOverrides": {}
  },
  "projectType": "CleanArchitecture",
  "extensions": {}
}
```

## Detailed Usage

### Initialize Configuration

#### Interactive Setup
```powershell
qcli config init --interactive
```

**Example Interactive Session:**
```
📝 Let's set up your project configuration...
? Project name: MyEcommerce
? Root namespace: MyEcommerce
? Project description (optional): E-commerce platform
? Author name (optional): John Doe

📁 Configure project paths:
? Domain path: [src/Core/Domain] 
? Application path: [src/Core/Application]
? Persistence path: [src/Infrastructure/Persistence]
? WebApi path: [src/Apps/Api]

⚙️ Code generation settings:
? Generate tests? (Y/n) Y
? Generate permissions? (Y/n) Y  
? Generate domain events? (y/N) N

✅ QCLI initialized successfully!
```

#### Quick Setup
```powershell
# Initialize with defaults
qcli config init

# Initialize with specific template
qcli config init --template minimal

# Initialize with custom path
qcli config init --path custom-config.json
```

### View Configuration

#### Show All Settings
```powershell
qcli config show
```

**Example Output:**
```
┌───────────────────────────┬────────────────────────────────────┐
│ Setting                   │ Value                              │
├───────────────────────────┼────────────────────────────────────┤
│ Project Type              │ CleanArchitecture                  │
│ Project Name              │ MyEcommerce                        │
│ Root Namespace            │ MyEcommerce                        │
│ Domain Path               │ src/Core/Domain                    │
│ Application Path          │ src/Core/Application               │
│ Persistence Path          │ src/Infrastructure/Persistence     │
│ API Path                  │ src/Apps/Api                       │
│ Default Entity Type       │ Audited                            │
│ Generate Events           │ False                              │
│ Generate Mapping Profiles │ False                              │
│ Generate Permissions      │ True                               │
│ Generate Tests            │ True                               │
└───────────────────────────┴────────────────────────────────────┘
```

#### Show Specific Configuration
```powershell
# View specific config file
qcli config show --path production.json

# View global configuration
qcli config show --global
```

### Get Configuration Values

```powershell
# Get specific value
qcli config get paths.domainPath
# Output: src/Core/Domain

# Get nested values
qcli config get codeGeneration.generateTests
# Output: true

# Get project information
qcli config get project.name
# Output: MyEcommerce
```

### Set Configuration Values

```powershell
# Set project paths
qcli config set paths.domainPath "src/Domain"
qcli config set paths.applicationPath "src/Application"

# Set code generation options
qcli config set codeGeneration.generateEvents true
qcli config set codeGeneration.defaultEntityType "FullyAudited"

# Set project information
qcli config set project.name "NewProjectName"
qcli config set project.namespace "Company.NewProject"
```

### Generate Sample Configuration

```powershell
# Generate sample config with all options
qcli config sample

# Generate to specific path
qcli config sample --path sample-config.json
```

**Generated Sample:**
```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json",
  "version": "1.0",
  "project": {
    "name": "SampleProject",
    "namespace": "SampleProject",
    "description": "A sample Clean Architecture project",
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
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": true,
    "generateTests": true
  },
  "templates": {
    "defaultTemplate": "clean-architecture",
    "customTemplatesPath": "templates",
    "enableCustomTemplates": false,
    "templateOverrides": {}
  },
  "projectType": "CleanArchitecture",
  "extensions": {}
}
```

## Configuration Options Reference

### Project Settings

| Setting | Type | Description | Default |
|---------|------|-------------|---------|
| `project.name` | string | Project name | Current directory name |
| `project.namespace` | string | Root namespace | Project name |
| `project.description` | string | Project description | "" |
| `project.author` | string | Author name | "" |
| `project.version` | string | Project version | "1.0.0" |

### Path Configuration

| Setting | Type | Description | Default |
|---------|------|-------------|---------|
| `paths.domainPath` | string | Domain layer path | "src/Core/Domain" |
| `paths.applicationPath` | string | Application layer path | "src/Core/Application" |
| `paths.persistencePath` | string | Persistence layer path | "src/Infrastructure/Persistence" |
| `paths.apiPath` | string | API layer path | "src/Apps/Api" |
| `paths.testsPath` | string | Tests root path | "tests" |

### Code Generation Settings

| Setting | Type | Description | Default |
|---------|------|-------------|---------|
| `codeGeneration.defaultEntityType` | string | Default entity type (`Entity`, `Audited`, `FullyAudited`) | "Audited" |
| `codeGeneration.generateEvents` | boolean | Generate domain events | false |
| `codeGeneration.generateMappingProfiles` | boolean | Generate Mapster profiles | false |
| `codeGeneration.generatePermissions` | boolean | Generate authorization permissions | true |
| `codeGeneration.generateTests` | boolean | Generate unit and integration tests | true |

### Template Settings

| Setting | Type | Description | Default |
|---------|------|-------------|---------|
| `templates.defaultTemplate` | string | Default project template | "clean-architecture" |
| `templates.customTemplatesPath` | string | Custom templates directory | "templates" |
| `templates.enableCustomTemplates` | boolean | Enable custom template support | false |
| `templates.templateOverrides` | object | Template override mappings | {} |

## Configuration Discovery

QCLI searches for configuration files in this order:

1. **Specified path** (via `--path` option)
2. **Current directory** (`qcli.json` or `quillysoft-cli.json`)
3. **Parent directories** (walking up the tree)
4. **Global configuration** (user home directory)
5. **Default configuration** (built-in defaults)

## Environment-Specific Configuration

### Development
```json
{
  "codeGeneration": {
    "generateTests": true,
    "generateEvents": true
  }
}
```

### Production
```json
{
  "codeGeneration": {
    "generateTests": false,
    "generateEvents": false
  }
}
```

### Team Configuration
```json
{
  "project": {
    "namespace": "Company.ProductName"
  },
  "paths": {
    "domainPath": "src/Domain",
    "applicationPath": "src/Application"
  },
  "codeGeneration": {
    "defaultEntityType": "FullyAudited",
    "generatePermissions": true
  }
}
```

## Examples

### E-commerce Project Setup
```powershell
# Initialize for e-commerce
qcli config init

# Configure for audit-heavy business
qcli config set codeGeneration.defaultEntityType "FullyAudited"
qcli config set codeGeneration.generateEvents true

# Set custom namespace
qcli config set project.namespace "ECommerce.Core"
```

### Microservice Configuration
```powershell
# Initialize with minimal template
qcli config init --template minimal

# Configure for microservice paths
qcli config set paths.domainPath "src/Domain"
qcli config set paths.applicationPath "src/Application"
qcli config set paths.apiPath "src/WebApi"

# Disable some features for microservices
qcli config set codeGeneration.generateEvents false
```

### Legacy Integration
```powershell
# Configure for existing project structure
qcli config set paths.domainPath "Business/Entities"
qcli config set paths.applicationPath "Business/Services"
qcli config set paths.persistencePath "Data/Configurations"

# Match existing patterns
qcli config set project.namespace "LegacyApp.Business"
```

## Validation

QCLI automatically validates configuration:

- **Path validation**: Ensures configured paths exist or can be created
- **Namespace validation**: Validates C# namespace conventions
- **Type validation**: Ensures entity types are valid
- **Template validation**: Verifies template availability

## Troubleshooting

### Configuration Not Found
```powershell
# Check current configuration location
qcli config show

# Initialize if missing
qcli config init
```

### Invalid Configuration
```powershell
# Regenerate with defaults
qcli config init --force

# Generate sample to compare
qcli config sample --path reference.json
```

### Path Issues
```powershell
# Reset to defaults
qcli config set paths.domainPath "src/Core/Domain"
qcli config set paths.applicationPath "src/Core/Application"
```

---

**Related Documentation:**
- [Paths Configuration](../configuration/paths.md) - Detailed path customization
- [Code Generation Settings](../configuration/code-generation.md) - Generation control
- [Templates](../configuration/templates.md) - Template system configuration
