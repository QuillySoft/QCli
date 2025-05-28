# Configuration Guide

QCLI uses a comprehensive configuration system to customize code generation, project structure, and tool behavior. This guide covers all configuration options and how to manage them effectively.

## Configuration File

QCLI stores its configuration in a `quillysoft-cli.json` file. The tool automatically searches for this file in the current directory and parent directories, making it easy to have project-specific configurations.

### File Location Priority

1. **Specified path**: Via `--config` option
2. **Current directory**: `./quillysoft-cli.json`
3. **Parent directories**: Searches upward until found
4. **Default configuration**: Auto-generated if no file found

### Creating Configuration

```bash
# Initialize with default settings
qcli config init

# Initialize with interactive prompts
qcli config init --interactive

# Generate sample configuration file
qcli config sample
```

## Configuration Structure

The configuration file has several main sections:

```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json",
  "version": "1.0",
  "projectType": "CleanArchitecture",
  "project": { /* Project metadata */ },
  "paths": { /* File system paths */ },
  "codeGeneration": { /* Code generation settings */ },
  "templates": { /* Template configuration */ },
  "extensions": { /* Custom extensions */ }
}
```

## Project Information

Configure basic project metadata that affects generated code and documentation.

```json
{
  "project": {
    "name": "MyECommerce",
    "namespace": "MyECommerce",
    "description": "E-commerce platform built with Clean Architecture",
    "author": "Development Team",
    "version": "1.0.0"
  }
}
```

### Project Settings

| Setting | Description | Default | Example |
|---------|-------------|---------|---------|
| `name` | Project name used in generated files | Directory name | `"MyECommerce"` |
| `namespace` | Root namespace for all generated code | Project name | `"MyECommerce"` |
| `description` | Project description for documentation | Empty | `"E-commerce platform"` |
| `author` | Author name for generated file headers | Empty | `"Development Team"` |
| `version` | Project version for documentation | `"1.0.0"` | `"2.1.0"` |

## Path Configuration

Define where QCLI should generate different types of files within your project structure.

```json
{
  "paths": {
    "rootPath": "C:\\Projects\\MyECommerce",
    "apiPath": "src\\Apps\\Api",
    "applicationPath": "src\\Core\\Application",
    "domainPath": "src\\Core\\Domain",
    "persistencePath": "src\\Infra\\Persistence",
    "applicationTestsPath": "tests\\Application\\ApplicationTests",
    "integrationTestsPath": "tests\\Infra\\InfraTests\\Controllers",
    "controllersPath": "src\\Apps\\Api\\Controllers"
  }
}
```

### Path Settings

| Setting | Description | Default | Generated Files |
|---------|-------------|---------|-----------------|
| `rootPath` | Project root directory | Current directory | Base for all relative paths |
| `apiPath` | WebAPI project location | `src\\Apps\\Api` | Controllers, startup files |
| `applicationPath` | Application layer location | `src\\Core\\Application` | Commands, queries, handlers |
| `domainPath` | Domain layer location | `src\\Core\\Domain` | Entities, value objects, events |
| `persistencePath` | Persistence layer location | `src\\Infra\\Persistence` | Entity configurations, DbContext |
| `applicationTestsPath` | Application tests location | `tests\\Application\\ApplicationTests` | Unit tests for commands/queries |
| `integrationTestsPath` | Integration tests location | `tests\\Infra\\InfraTests\\Controllers` | Controller integration tests |
| `controllersPath` | Controllers directory | `src\\Apps\\Api\\Controllers` | API controllers |

### Path Auto-Detection

QCLI automatically detects common project structures:

```bash
# Auto-detect paths based on current project
qcli config init

# View detected paths
qcli config show
```

**Supported Project Structures:**
- **Clean Architecture**: `src/Core`, `src/Apps`, `src/Infra` structure
- **Onion Architecture**: Layered directory structure
- **Standard .NET**: Traditional project layout
- **Minimal Structure**: Simple folder organization

## Code Generation Settings

Control what gets generated when you create entities and operations.

```json
{
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

### Generation Options

| Setting | Description | Default | Impact |
|---------|-------------|---------|---------|
| `defaultEntityType` | Default base entity type | `"Audited"` | Determines entity inheritance |
| `generateEvents` | Generate domain events | `true` | Creates event classes and handlers |
| `generateMappingProfiles` | Generate Mapster profiles | `true` | Creates mapping configurations |
| `generatePermissions` | Generate permission constants | `true` | Creates authorization definitions |
| `generateTests` | Generate unit tests | `true` | Creates test classes and fixtures |

### Entity Types

Choose the appropriate base entity type for your domain models:

**Audited** (Default)
- Includes: `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- Use for: Standard business entities requiring audit trails

**FullyAudited**
- Includes: All Audited fields plus `DeletedAt`, `DeletedBy`, `IsDeleted`
- Use for: Entities requiring soft deletion

**Basic**
- Includes: Only `Id` field
- Use for: Simple lookup tables, minimal entities

### Event Generation

When `generateEvents` is enabled, QCLI creates:

```csharp
// Domain events
public sealed class ProductCreatedEvent : INotification
{
    public Guid ProductId { get; }
}

// Event handlers
public sealed class Handler : INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Event handling logic
    }
}
```

### Permission Generation

When `generatePermissions` is enabled, QCLI creates:

```csharp
public static partial class Permissions
{
    public static class Products
    {
        public static class Actions
        {
            public const string View = "Permissions:Products:View";
            public const string Create = "Permissions:Products:Create";
            public const string Update = "Permissions:Products:Update";
            public const string Delete = "Permissions:Products:Delete";
        }
    }
}
```

## Template Configuration

Configure which templates to use and customize template behavior.

```json
{
  "templates": {
    "defaultTemplate": "clean-architecture",
    "customTemplatesPath": "templates",
    "enableCustomTemplates": false,
    "templateOverrides": {
      "entity": "custom-entity",
      "command": "enhanced-command"
    }
  }
}
```

### Template Settings

| Setting | Description | Default | Options |
|---------|-------------|---------|---------|
| `defaultTemplate` | Default template for new projects | `"clean-architecture"` | `clean-architecture`, `minimal`, `ddd` |
| `customTemplatesPath` | Path to custom templates | `"templates"` | Any valid directory path |
| `enableCustomTemplates` | Allow custom template usage | `false` | `true`, `false` |
| `templateOverrides` | Override specific templates | `{}` | Key-value pairs of template mappings |

### Available Templates

**clean-architecture** (Default)
- Complete Clean Architecture implementation
- Includes all layers and common patterns
- Best for: Most business applications

**minimal**
- Minimal setup with basic structure
- Reduced boilerplate and dependencies
- Best for: Simple projects, prototypes

**ddd**
- Domain-Driven Design focused
- Rich domain models and aggregates
- Best for: Complex business domains

## Project Types

Configure the overall architecture pattern for your project.

```json
{
  "projectType": "CleanArchitecture"
}
```

### Supported Project Types

| Type | Description | Structure |
|------|-------------|-----------|
| `CleanArchitecture` | Clean Architecture pattern | Core/Apps/Infrastructure separation |
| `OnionArchitecture` | Onion Architecture pattern | Layered circular dependencies |
| `MinimalApi` | Minimal API structure | Simplified project layout |
| `Microservice` | Microservice architecture | Service-oriented structure |

## Extension Points

Add custom configuration for plugins and extensions.

```json
{
  "extensions": {
    "logging": {
      "provider": "Serilog",
      "level": "Information"
    },
    "database": {
      "provider": "SqlServer",
      "connectionStringName": "DefaultConnection"
    },
    "customSettings": {
      "enableFeatureX": true,
      "apiVersion": "v2"
    }
  }
}
```

## Configuration Management

### Viewing Configuration

```bash
# Show all configuration settings
qcli config show

# Get specific setting
qcli config get paths.applicationPath
qcli config get codeGeneration.generateEvents
```

### Updating Configuration

```bash
# Set specific setting
qcli config set codeGeneration.generateTests false
qcli config set paths.apiPath "src/WebApi"

# Initialize new configuration
qcli config init --force
```

### Environment-Specific Configuration

Create environment-specific configurations:

```bash
# Development configuration
quillysoft-cli.dev.json

# Production configuration
quillysoft-cli.prod.json

# Use specific configuration
qcli add Product --all --config quillysoft-cli.dev.json
```

## Configuration Examples

### E-Commerce Project

```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json",
  "version": "1.0",
  "projectType": "CleanArchitecture",
  "project": {
    "name": "ECommerceApp",
    "namespace": "ECommerce",
    "description": "Modern e-commerce platform",
    "author": "E-Commerce Team"
  },
  "paths": {
    "rootPath": "C:\\Projects\\ECommerce",
    "applicationPath": "src\\Application",
    "domainPath": "src\\Domain",
    "persistencePath": "src\\Infrastructure\\Persistence",
    "controllersPath": "src\\WebApi\\Controllers"
  },
  "codeGeneration": {
    "defaultEntityType": "FullyAudited",
    "generateEvents": true,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

### Microservice Configuration

```json
{
  "projectType": "Microservice",
  "project": {
    "name": "OrderService",
    "namespace": "OrderService"
  },
  "paths": {
    "applicationPath": "src\\Application",
    "domainPath": "src\\Domain",
    "persistencePath": "src\\Infrastructure"
  },
  "codeGeneration": {
    "generateEvents": true,
    "generateMappingProfiles": false,
    "generateTests": true
  },
  "templates": {
    "defaultTemplate": "microservice"
  }
}
```

### Minimal API Project

```json
{
  "projectType": "MinimalApi",
  "paths": {
    "apiPath": "src",
    "applicationPath": "src\\Features",
    "domainPath": "src\\Models"
  },
  "codeGeneration": {
    "defaultEntityType": "Basic",
    "generateEvents": false,
    "generateMappingProfiles": false,
    "generatePermissions": false,
    "generateTests": false
  },
  "templates": {
    "defaultTemplate": "minimal"
  }
}
```

## Configuration Validation

QCLI automatically validates configuration settings:

```bash
# Run configuration health check
qcli doctor

# Check for configuration issues
qcli config validate
```

### Common Validation Issues

| Issue | Description | Solution |
|-------|-------------|----------|
| Invalid paths | Path does not exist | Create directory or update path |
| Missing permissions | Cannot write to path | Check file system permissions |
| Schema mismatch | Configuration format error | Regenerate with `qcli config init` |
| Template not found | Custom template missing | Check template path or disable custom templates |

## Schema Support

QCLI provides JSON schema for IntelliSense and validation in your IDE:

```json
{
  "$schema": "https://quillysoft.com/schemas/qcli-config.json"
}
```

### IDE Integration

**Visual Studio Code**
- Automatic schema detection
- IntelliSense for configuration properties
- Validation warnings and errors

**Visual Studio**
- JSON schema validation
- Property completion
- Error highlighting

## Migration and Upgrades

When upgrading QCLI, configuration may need migration:

```bash
# Check for configuration updates
qcli config check-migration

# Migrate configuration to new version
qcli config migrate

# Backup before migration
qcli config backup
```

## Best Practices

### Project Organization
- Keep configuration at project root
- Use descriptive project names and namespaces
- Set appropriate entity types for your domain

### Path Management
- Use relative paths for portability
- Follow established project conventions
- Verify paths exist before generation

### Code Generation
- Enable tests for quality assurance
- Use events for loosely coupled architecture
- Generate permissions for security

### Template Usage
- Start with default templates
- Customize only when necessary
- Document custom template decisions

## Troubleshooting

### Configuration Not Found
```bash
# Verify configuration location
qcli config show

# Initialize if missing
qcli config init
```

### Invalid Configuration
```bash
# Check configuration validity
qcli doctor

# Regenerate configuration
qcli config init --force
```

### Path Issues
```bash
# Verify paths exist
qcli config validate

# Auto-detect paths
qcli config init --auto-detect
```

## Related Documentation

- [Path Configuration](./paths.md) - Detailed path configuration guide
- [Code Generation Settings](./code-generation.md) - Code generation options
- [Template Configuration](./templates.md) - Template system configuration
- [CLI Reference](../reference/cli-reference.md) - Complete command reference
