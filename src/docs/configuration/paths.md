# Path Configuration Guide

QCLI uses a flexible path configuration system to adapt to different project structures and conventions. This guide explains how to configure, customize, and manage file generation paths effectively.

## Overview

The path configuration determines where QCLI generates different types of files within your project. The system supports both absolute and relative paths, with automatic detection for common project structures.

## Path Structure

```json
{
  "paths": {
    "rootPath": "C:\\Projects\\MyApp",
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

## Path Configuration Reference

### Root Path
**Property**: `rootPath`  
**Description**: Base directory for the entire project  
**Default**: Current working directory  
**Example**: `"C:\\Projects\\ECommerce"`

The root path serves as the base for all relative paths. All other paths are resolved relative to this directory.

```bash
# Set root path explicitly
qcli config set paths.rootPath "C:\Projects\MyApp"

# Auto-detect root path
qcli config init --auto-detect
```

### API Path
**Property**: `apiPath`  
**Description**: Location of the Web API project  
**Default**: `"src\\Apps\\Api"`  
**Generated Files**: Startup files, configuration, middleware

```json
{
  "paths": {
    "apiPath": "src\\WebApi"
  }
}
```

**File Generation Structure**:
```
src/WebApi/
├── Controllers/          (controllersPath)
├── Configuration/
├── Middleware/
└── Program.cs
```

### Application Path
**Property**: `applicationPath`  
**Description**: Application layer location (commands, queries, handlers)  
**Default**: `"src\\Core\\Application"`  
**Generated Files**: Commands, queries, DTOs, mappings, events

```json
{
  "paths": {
    "applicationPath": "src\\Application"
  }
}
```

**File Generation Structure**:
```
src/Application/
├── Products/
│   ├── Commands/
│   │   ├── CreateProduct/
│   │   ├── UpdateProduct/
│   │   └── DeleteProduct/
│   ├── Queries/
│   │   ├── GetProducts/
│   │   └── GetProductDetails/
│   ├── Events/
│   └── Mapping/
└── Common/
```

### Domain Path
**Property**: `domainPath`  
**Description**: Domain layer location (entities, value objects, events)  
**Default**: `"src\\Core\\Domain"`  
**Generated Files**: Entities, value objects, domain events, permissions

```json
{
  "paths": {
    "domainPath": "src\\Domain"
  }
}
```

**File Generation Structure**:
```
src/Domain/
├── Products/
│   ├── Product.cs           (Entity)
│   ├── ProductValueObjects.cs
│   └── ProductEvents.cs
├── PermissionsConstants/
│   └── ProductPermissions.cs
└── Common/
```

### Persistence Path
**Property**: `persistencePath`  
**Description**: Infrastructure persistence layer location  
**Default**: `"src\\Infra\\Persistence"`  
**Generated Files**: Entity configurations, migrations, repositories

```json
{
  "paths": {
    "persistencePath": "src\\Infrastructure\\Persistence"
  }
}
```

**File Generation Structure**:
```
src/Infrastructure/Persistence/
├── Configurations/
│   └── Tenants/
│       └── Products/
│           └── ProductEntityConfiguration.cs
├── Migrations/
└── ApplicationDbContext.cs
```

### Controllers Path
**Property**: `controllersPath`  
**Description**: API controllers location  
**Default**: `"src\\Apps\\Api\\Controllers"`  
**Generated Files**: API controllers

```json
{
  "paths": {
    "controllersPath": "src\\WebApi\\Controllers"
  }
}
```

**File Generation Structure**:
```
src/WebApi/Controllers/
├── ProductsController.cs
├── OrdersController.cs
└── CustomersController.cs
```

### Test Paths

#### Application Tests Path
**Property**: `applicationTestsPath`  
**Description**: Application layer unit tests  
**Default**: `"tests\\Application\\ApplicationTests"`  
**Generated Files**: Command tests, query tests, handler tests

```json
{
  "paths": {
    "applicationTestsPath": "tests\\UnitTests\\Application"
  }
}
```

#### Integration Tests Path
**Property**: `integrationTestsPath`  
**Description**: Integration and controller tests  
**Default**: `"tests\\Infra\\InfraTests\\Controllers"`  
**Generated Files**: Controller integration tests, database tests

```json
{
  "paths": {
    "integrationTestsPath": "tests\\IntegrationTests"
  }
}
```

**Test Structure**:
```
tests/
├── UnitTests/
│   └── Application/
│       └── Products/
│           ├── Commands/
│           └── Queries/
└── IntegrationTests/
    └── Controllers/
        └── ProductsControllerTests.cs
```

## Auto-Detection

QCLI automatically detects common project structures and suggests appropriate paths.

### Supported Project Patterns

#### Clean Architecture
```
src/
├── Core/
│   ├── Application/
│   └── Domain/
├── Apps/
│   └── Api/
└── Infra/
    └── Persistence/
```

#### Onion Architecture
```
src/
├── Application/
├── Domain/
├── Infrastructure/
└── WebApi/
```

#### Standard .NET
```
src/
├── ProjectName.Application/
├── ProjectName.Domain/
├── ProjectName.Infrastructure/
└── ProjectName.WebApi/
```

#### Minimal Structure
```
src/
├── Models/
├── Services/
├── Controllers/
└── Data/
```

### Auto-Detection Process

```bash
# Run auto-detection
qcli config init --auto-detect

# View detected paths
qcli config show

# Customize detected paths
qcli config init --interactive
```

**Detection Logic**:
1. Scan current directory structure
2. Identify common patterns (src/, Core/, Apps/, etc.)
3. Match against known templates
4. Suggest appropriate path configuration
5. Allow user customization

## Custom Path Configuration

### Interactive Configuration

```bash
# Start interactive path configuration
qcli config init --interactive

# Example prompts:
# Root Path: C:\Projects\MyApp
# Application Path (relative to root): src/Application
# Domain Path (relative to root): src/Domain
# API Path (relative to root): src/WebApi
```

### Programmatic Configuration

```bash
# Set individual paths
qcli config set paths.applicationPath "src/Core/Application"
qcli config set paths.domainPath "src/Core/Domain"
qcli config set paths.controllersPath "src/WebApi/Controllers"

# Set multiple paths at once
qcli config set paths '{
  "applicationPath": "src/Application",
  "domainPath": "src/Domain",
  "persistencePath": "src/Infrastructure"
}'
```

## Path Validation

QCLI validates paths during configuration and generation:

### Validation Checks

1. **Path Existence**: Verifies directories exist
2. **Write Permissions**: Ensures QCLI can create files
3. **Path Conflicts**: Checks for overlapping paths
4. **Relative Path Resolution**: Validates relative paths resolve correctly

```bash
# Validate current path configuration
qcli config validate

# Check paths during generation
qcli doctor --check-paths
```

### Common Validation Issues

| Issue | Description | Solution |
|-------|-------------|----------|
| Path not found | Directory doesn't exist | Create directory or update path |
| Permission denied | Cannot write to directory | Check file system permissions |
| Invalid relative path | Path doesn't resolve correctly | Use absolute path or fix relative reference |
| Circular reference | Paths reference each other | Restructure path configuration |

## Environment-Specific Paths

Configure different paths for different environments:

### Development Configuration
```json
{
  "paths": {
    "rootPath": "C:\\Dev\\MyApp",
    "applicationPath": "src\\Application",
    "persistencePath": "src\\Infrastructure"
  }
}
```

### Production Configuration
```json
{
  "paths": {
    "rootPath": "/opt/myapp",
    "applicationPath": "src/Application",
    "persistencePath": "src/Infrastructure"
  }
}
```

### Team Configuration
```json
{
  "paths": {
    "rootPath": ".",
    "applicationPath": "Application",
    "domainPath": "Domain",
    "persistencePath": "Infrastructure"
  }
}
```

## Advanced Path Features

### Path Variables

Use variables in path configuration for dynamic resolution:

```json
{
  "paths": {
    "rootPath": "${PROJECT_ROOT}",
    "applicationPath": "${SRC_DIR}/Application",
    "domainPath": "${SRC_DIR}/Domain"
  }
}
```

### Conditional Paths

Configure paths based on project type:

```json
{
  "paths": {
    "applicationPath": "src/Core/Application",
    "domainPath": "src/Core/Domain",
    "apiPath": "src/Apps/Api"
  },
  "pathOverrides": {
    "Microservice": {
      "applicationPath": "src/Application",
      "apiPath": "src/Api"
    },
    "MinimalApi": {
      "applicationPath": "Features",
      "domainPath": "Models"
    }
  }
}
```

## Migration and Updates

### Path Migration

When updating project structure, migrate paths:

```bash
# Backup current configuration
qcli config backup

# Update paths
qcli config set paths.applicationPath "new/path/Application"

# Migrate existing files (if supported)
qcli migrate --from-path "old/path" --to-path "new/path"
```

### Bulk Path Updates

```bash
# Update all paths with new base
qcli config update-paths --base-path "src" --new-base "source"

# Convert to relative paths
qcli config normalize-paths --relative

# Convert to absolute paths
qcli config normalize-paths --absolute
```

## Best Practices

### Path Organization

1. **Use Standard Conventions**: Follow established patterns for your architecture
2. **Keep Paths Relative**: Use relative paths for team collaboration
3. **Group Related Files**: Place related files in logical directory structures
4. **Avoid Deep Nesting**: Keep directory structures manageable

### Configuration Management

1. **Version Control**: Include path configuration in version control
2. **Team Alignment**: Ensure all team members use consistent paths
3. **Documentation**: Document custom path decisions
4. **Regular Validation**: Periodically validate path configuration

### Project Structure

1. **Separate Concerns**: Use different paths for different layers
2. **Test Organization**: Keep test paths aligned with source structure
3. **Build Integration**: Ensure paths work with build systems
4. **Deployment Compatibility**: Consider deployment path requirements

## Common Project Configurations

### Enterprise Application
```json
{
  "paths": {
    "rootPath": ".",
    "applicationPath": "src/Core/Application",
    "domainPath": "src/Core/Domain",
    "persistencePath": "src/Infrastructure/Persistence",
    "apiPath": "src/Presentation/WebApi",
    "controllersPath": "src/Presentation/WebApi/Controllers",
    "applicationTestsPath": "tests/Core/Application.Tests",
    "integrationTestsPath": "tests/Infrastructure/Integration.Tests"
  }
}
```

### Microservice
```json
{
  "paths": {
    "rootPath": ".",
    "applicationPath": "src/Application",
    "domainPath": "src/Domain",
    "persistencePath": "src/Infrastructure",
    "apiPath": "src/Api",
    "controllersPath": "src/Api/Controllers",
    "applicationTestsPath": "tests/Application.Tests",
    "integrationTestsPath": "tests/Integration.Tests"
  }
}
```

### Modular Monolith
```json
{
  "paths": {
    "rootPath": ".",
    "applicationPath": "src/Modules/{ModuleName}/Application",
    "domainPath": "src/Modules/{ModuleName}/Domain",
    "persistencePath": "src/Modules/{ModuleName}/Infrastructure",
    "apiPath": "src/WebApi",
    "controllersPath": "src/WebApi/Controllers/{ModuleName}"
  }
}
```

## Troubleshooting

### Path Not Found Errors
```bash
# Check if paths exist
qcli config validate

# Create missing directories
mkdir -p $(qcli config get paths.applicationPath)
```

### Permission Issues
```bash
# Check permissions
ls -la $(qcli config get paths.rootPath)

# Fix permissions (Unix/Linux)
chmod -R 755 $(qcli config get paths.rootPath)
```

### Path Resolution Problems
```bash
# View resolved paths
qcli config show --resolved

# Test path resolution
qcli config test-paths
```

## Related Documentation

- [Configuration Guide](./configuration.md) - Main configuration overview
- [Code Generation Settings](./code-generation.md) - Configure what gets generated
- [Template Configuration](./templates.md) - Template-specific path settings
- [Project Setup](../quick-start.md) - Getting started with path configuration
