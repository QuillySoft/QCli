# 🩺 doctor Command

Diagnose and fix common issues in your QCLI project.

## Overview

The `doctor` command performs comprehensive health checks on your QCLI project, identifying configuration issues, missing dependencies, and structural problems. It can automatically fix many common issues or provide guidance for manual resolution.

## Syntax

```powershell
qcli doctor [options]
```

## Options

| Option | Description | Example |
|--------|-------------|---------|
| `--fix` | Automatically fix issues when possible | `qcli doctor --fix` |

## Diagnostic Checks

### 🔧 Configuration Health
- **Configuration file existence** (`qcli.json`)
- **Configuration syntax validation**
- **Path verification** (domain, application, persistence)
- **Schema compliance**
- **Namespace validation**

### 📁 Project Structure
- **Directory structure validation**
- **Required paths existence**
- **File permissions**
- **Generated file integrity**

### 🏗️ Build Health
- **Project compilation status**
- **Missing dependencies**
- **Assembly references**
- **Package versions**

### 🔗 Integration Status
- **DbContext integration**
- **Entity configurations**
- **Permission registrations**
- **Migration status**

## Usage Examples

### Basic Health Check
```powershell
qcli doctor
```

**Example Output:**
```
🩺 QCLI Project Health Check

Checking configuration...                    ✅ PASSED
Checking project structure...                ✅ PASSED  
Checking build status...                     ⚠️  WARNINGS
Checking integrations...                     ❌ FAILED

📋 Diagnostic Results:

✅ Configuration file found and valid
✅ All configured paths exist
✅ Project structure is correct
✅ Permissions are properly configured

⚠️  Build Warnings (2):
  - Package 'Microsoft.EntityFrameworkCore' could be updated (8.0.0 → 8.0.1)
  - XML documentation file not found

❌ Integration Issues (1):
  - Entity 'Product' not registered in DbContext
    Fix: Add 'public DbSet<Product> Products { get; set; }' to ApplicationDbContext

🔧 Run 'qcli doctor --fix' to auto-fix available issues
```

### Auto-Fix Issues
```powershell
qcli doctor --fix
```

**Example Output:**
```
🩺 QCLI Project Health Check with Auto-Fix

Checking and fixing issues...

✅ Fixed: Created missing directory 'src/Core/Domain'
✅ Fixed: Updated invalid path configuration
✅ Fixed: Regenerated corrupted entity configuration
⚠️  Manual fix required: Entity not registered in DbContext

📋 Summary:
  - 3 issues fixed automatically
  - 1 issue requires manual intervention
  - 0 critical errors remaining

📝 Manual fixes needed:
  1. Add entity to DbContext (see integration guide)
```

## Detailed Diagnostic Categories

### Configuration Diagnostics

#### ✅ Healthy Configuration
```
Configuration Health: ✅ PASSED
├── ✅ Configuration file exists (qcli.json)
├── ✅ Valid JSON syntax
├── ✅ Schema compliance
├── ✅ All paths exist
├── ✅ Valid namespace format
└── ✅ Generation settings valid
```

#### ❌ Configuration Issues
```
Configuration Health: ❌ FAILED
├── ❌ Configuration file not found
├── ⚠️  Invalid JSON syntax at line 15
├── ❌ Path not found: src/NonExistent/Domain
├── ⚠️  Namespace contains invalid characters
└── ❌ Invalid entity type: 'CustomType'

🔧 Auto-fixable issues:
  - Create missing paths
  - Reset invalid settings to defaults

📝 Manual fixes required:
  - Fix JSON syntax errors
  - Update invalid namespace
```

### Project Structure Diagnostics

#### ✅ Healthy Structure
```
Project Structure: ✅ PASSED
├── ✅ Domain layer structure correct
├── ✅ Application layer structure correct
├── ✅ Infrastructure layer structure correct
├── ✅ API layer structure correct
└── ✅ Test structure correct
```

#### ❌ Structure Issues
```
Project Structure: ❌ FAILED
├── ❌ Missing directory: src/Core/Domain
├── ⚠️  Unexpected files in domain layer
├── ❌ Application commands not properly organized
└── ⚠️  Test files missing for some entities

🔧 Auto-fixable issues:
  - Create missing directories
  - Reorganize misplaced files

📝 Manual fixes required:
  - Review and move unexpected files
  - Generate missing test files
```

### Build Diagnostics

#### ✅ Healthy Build
```
Build Health: ✅ PASSED
├── ✅ Project compiles successfully
├── ✅ All packages up to date
├── ✅ No missing references
└── ✅ All tests pass
```

#### ❌ Build Issues
```
Build Health: ❌ FAILED
├── ❌ Compilation errors (3)
├── ⚠️  Package updates available (2)
├── ❌ Missing assembly reference
└── ⚠️  Test failures (1)

📝 Compilation Errors:
  1. CS0246: Type 'Product' not found in CreateProductCommand.cs
  2. CS0234: Namespace 'Domain.Entities' does not exist
  3. CS1061: Missing 'Products' property in DbContext

🔧 Suggested fixes:
  - Add missing using statements
  - Register entity in DbContext
  - Verify entity file exists
```

### Integration Diagnostics

#### ✅ Healthy Integration
```
Integration Status: ✅ PASSED
├── ✅ All entities registered in DbContext
├── ✅ Entity configurations applied
├── ✅ Permissions properly registered
├── ✅ Migrations up to date
└── ✅ Controllers properly wired
```

#### ❌ Integration Issues
```
Integration Status: ❌ FAILED
├── ❌ Entity 'Product' not in DbContext
├── ❌ Missing entity configuration registration
├── ⚠️  Permissions not registered
└── ❌ Pending migration required

📝 Required Manual Fixes:

1. Register Entity in DbContext:
   Add to ApplicationDbContext:
   public DbSet<Product> Products { get; set; }

2. Register Entity Configuration:
   Add to OnModelCreating:
   modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());

3. Register Permissions:
   Add to PermissionsProvider:
   Permissions.Products.Actions.View,
   Permissions.Products.Actions.Create,
   // ... etc

4. Create Migration:
   dotnet ef migrations add AddProduct
   dotnet ef database update
```

## Issue Categories

### 🔴 Critical Issues
Issues that prevent QCLI from functioning:
- Missing configuration file
- Invalid JSON syntax
- Missing required paths
- Compilation failures

### 🟡 Warnings
Issues that may cause problems:
- Outdated packages
- Missing documentation
- Suboptimal configurations
- Performance concerns

### 🔵 Suggestions
Recommendations for improvement:
- Code organization improvements
- Best practice violations
- Optimization opportunities
- Maintenance reminders

## Auto-Fix Capabilities

The `--fix` option can automatically resolve:

### ✅ Automatically Fixable
- **Missing directories**: Creates required project structure
- **Invalid paths**: Resets to valid default paths
- **Corrupted configurations**: Regenerates with defaults
- **Missing entity configurations**: Recreates EF configurations
- **File organization**: Moves files to correct locations

### 📝 Manual Fix Required
- **DbContext registration**: Must be added manually
- **Permission registration**: Requires code modification
- **Migration creation**: Needs developer review
- **Compilation errors**: Requires code analysis
- **Custom business logic**: Can't be automated

## Integration with Other Commands

### Doctor + Config
```powershell
# Check configuration health
qcli doctor

# Fix configuration issues
qcli config init --force
```

### Doctor + Generate
```powershell
# Diagnose after generation
qcli add Product --all
qcli doctor

# Fix integration issues
qcli doctor --fix
```

### Doctor + Build
```powershell
# Check before building
qcli doctor
dotnet build

# Diagnose build failures
qcli doctor --fix
```

## Continuous Health Monitoring

### Pre-commit Checks
```powershell
# Add to pre-commit hook
qcli doctor
if ($LASTEXITCODE -ne 0) { 
    Write-Error "Health check failed. Fix issues before committing."
    exit 1
}
```

### CI/CD Pipeline
```yaml
# GitHub Actions example
- name: QCLI Health Check
  run: |
    qcli doctor
    if [ $? -ne 0 ]; then
      echo "Health check failed"
      exit 1
    fi
```

### Development Workflow
```powershell
# Daily development routine
qcli doctor               # Check project health
qcli add NewEntity --all  # Generate new code
qcli doctor --fix         # Fix any issues
dotnet build              # Verify build
dotnet test               # Run tests
```

## Common Issues and Solutions

### Configuration Issues

#### Missing Configuration
```
Issue: Configuration file not found
Solution: Run 'qcli init' to create configuration
```

#### Invalid Paths
```
Issue: Configured path does not exist: src/Invalid/Path
Fix: Update path or create directory
Auto-fix: Creates missing directories
```

### Build Issues

#### Missing Entity in DbContext
```
Issue: Entity 'Product' not registered in DbContext
Solution: Add 'public DbSet<Product> Products { get; set; }'
```

#### Compilation Errors
```
Issue: CS0246: Type 'Product' not found
Solution: Verify entity file exists and namespace is correct
```

### Integration Issues

#### Missing Permissions
```
Issue: Permissions not registered for entity 'Product'
Solution: Add permissions to PermissionsProvider
```

#### Pending Migrations
```
Issue: Migration required for new entity
Solution: Run 'dotnet ef migrations add AddProduct'
```

## Exit Codes

| Code | Description |
|------|-------------|
| `0` | All checks passed |
| `1` | Warnings found |
| `2` | Errors found |
| `3` | Critical issues found |

## Scripting with Doctor

### PowerShell Script
```powershell
$result = qcli doctor
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Project healthy" -ForegroundColor Green
} elseif ($LASTEXITCODE -eq 1) {
    Write-Host "⚠️ Warnings found" -ForegroundColor Yellow
    qcli doctor --fix
} else {
    Write-Host "❌ Errors found" -ForegroundColor Red
    exit $LASTEXITCODE
}
```

### Bash Script
```bash
#!/bin/bash
qcli doctor
case $? in
    0) echo "✅ Project healthy" ;;
    1) echo "⚠️ Warnings found"; qcli doctor --fix ;;
    *) echo "❌ Errors found"; exit 1 ;;
esac
```

## Best Practices

1. **Regular Checks**: Run `qcli doctor` regularly during development
2. **Auto-fix First**: Use `--fix` option to resolve simple issues automatically
3. **CI Integration**: Include health checks in build pipeline
4. **Team Standards**: Ensure all team members run doctor before commits
5. **Documentation**: Keep track of recurring issues for team training

---

**Related Documentation:**
- [Troubleshooting Guide](../advanced/troubleshooting.md) - Common issues and solutions
- [Integration Guide](../advanced/integration.md) - Complete integration steps
- [Configuration Guide](../configuration/configuration.md) - Configuration validation
