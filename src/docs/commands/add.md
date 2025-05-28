# 🎯 add Command

Generate complete CRUD operations for entities with full Clean Architecture implementation.

## Overview

The `add` command is the core functionality of QCLI, generating enterprise-grade CRUD operations following Clean Architecture, CQRS, and DDD patterns. In 30 seconds, it creates 20+ files with complete functionality.

## Syntax

```powershell
qcli add <EntityName> [options]
```

## Arguments

| Argument | Description | Required |
|----------|-------------|----------|
| `EntityName` | Name of the entity (singular form) | Yes |

## Options

### Operation Flags

| Option | Description | Example |
|--------|-------------|---------|
| `--all` | Generate all CRUD operations | `qcli add Product --all` |
| `--create` | Generate create operation only | `qcli add Product --create` |
| `--read` | Generate read operations (queries) | `qcli add Product --read` |
| `--update` | Generate update operation only | `qcli add Product --update` |
| `--delete` | Generate delete operation only | `qcli add Product --delete` |

### Entity Configuration

| Option | Description | Values | Default |
|--------|-------------|--------|---------|
| `--entity-type` | Entity base type | `Entity`, `Audited`, `FullyAudited` | `Audited` |

### Generation Control

| Option | Description | Example |
|--------|-------------|---------|
| `--no-tests` | Skip generating unit tests | `qcli add Product --all --no-tests` |
| `--no-permissions` | Skip generating permissions | `qcli add Product --all --no-permissions` |
| `--template` | Use specific template | `qcli add Product --all --template minimal` |
| `--output` | Specify output directory | `qcli add Product --all --output src/` |
| `--dry-run` | Preview without creating files | `qcli add Product --all --dry-run` |

## Entity Types Explained

### Entity (Basic)
```powershell
qcli add Category --all --entity-type Entity
```
- Basic entity with Id only
- No audit fields
- Use for: Reference data, lookup tables

**Generated Entity:**
```csharp
public class Category : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
}
```

### Audited (Recommended)
```powershell
qcli add Product --all --entity-type Audited
```
- Includes audit fields (CreatedBy, CreatedDate, etc.)
- Hard delete (permanent removal)
- Use for: Most business entities

**Generated Entity:**
```csharp
public class Product : AuditedEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    // Inherited: CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
}
```

### FullyAudited (Enterprise)
```powershell
qcli add Order --all --entity-type FullyAudited
```
- All audit fields + soft delete
- DeletedBy, DeletedDate fields
- Use for: Critical business data

**Generated Entity:**
```csharp
public class Order : FullyAuditedEntity<Guid>
{
    public string OrderNumber { get; set; } = string.Empty;
    // Inherited: CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, DeletedBy, DeletedDate, IsDeleted
}
```

## Generated Files Structure

### Domain Layer
```
src/Core/Domain/
├── Entities/
│   └── Product.cs                    # Domain entity
└── Permissions/
    └── ProductPermissions.cs         # Authorization permissions
```

### Application Layer
```
src/Core/Application/
└── Products/
    ├── Commands/
    │   ├── CreateProduct/
    │   │   ├── CreateProductCommand.cs
    │   │   └── CreateProductCommandValidator.cs
    │   ├── UpdateProduct/
    │   │   ├── UpdateProductCommand.cs
    │   │   └── UpdateProductCommandValidator.cs
    │   └── DeleteProduct/
    │       └── DeleteProductCommand.cs
    ├── Queries/
    │   ├── GetProduct/
    │   │   └── GetProductQuery.cs
    │   └── GetProducts/
    │       └── GetProductsQuery.cs
    └── DTOs/
        └── ProductForCreateUpdateDto.cs
```

### Infrastructure Layer
```
src/Infrastructure/
└── Persistence/
    └── Configurations/
        └── ProductEntityConfiguration.cs
```

### Presentation Layer
```
src/Apps/Api/
└── Controllers/
    └── ProductsController.cs
```

### Tests
```
tests/
├── Application/
│   └── Products/
│       ├── Commands/
│       │   └── CreateProductCommandTests.cs
│       └── Queries/
│           └── GetProductsQueryTests.cs
└── Integration/
    └── Controllers/
        └── ProductsControllerTests.cs
```

## Examples

### Basic E-commerce Setup
```powershell
# Product catalog
qcli add Product --all --entity-type Audited
qcli add Category --all --entity-type Audited

# Order management
qcli add Order --all --entity-type FullyAudited
qcli add OrderItem --all --entity-type Audited

# Customer data
qcli add Customer --all --entity-type FullyAudited
qcli add Address --all --entity-type Audited
```

### CRM System
```powershell
# Core entities
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type FullyAudited

# Sales pipeline
qcli add Lead --all --entity-type Audited
qcli add Opportunity --all --entity-type Audited
qcli add Deal --all --entity-type FullyAudited
```

### Selective Generation
```powershell
# Only create and read operations
qcli add Report --create --read

# Skip tests for simple entities
qcli add Status --all --no-tests --entity-type Entity

# Preview before generating
qcli add ComplexEntity --all --dry-run
```

## Generated Code Features

### 🏗️ Architecture Patterns
- **Clean Architecture** with proper layer separation
- **CQRS** for command/query responsibility segregation
- **Domain-Driven Design** with rich domain entities

### 🛡️ Security & Authorization
- **Permission-based authorization** on all operations
- **Nested permission structure** for enterprise scalability
- **Authorization attributes** on all commands/queries

### ⚡ Performance Optimizations
- **AsNoTracking()** for read-only queries
- **Projection to DTOs** to minimize data transfer
- **Pagination support** for large datasets

### 🧪 Testing Coverage
- **Unit tests** for all commands and queries
- **Integration tests** for API controllers
- **Validation tests** for DTOs and commands

### 📝 Code Quality
- **Consistent naming conventions**
- **XML documentation** for all public APIs
- **Proper error handling** with custom exceptions
- **Immutable DTOs** with validation rules

## Integration Steps

After generation, complete these steps:

### 1. Add to DbContext
```csharp
// Add to your DbContext
public DbSet<Product> Products { get; set; }
```

### 2. Register Entity Configuration
```csharp
// In OnModelCreating
modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
```

### 3. Add Permissions
```csharp
// Add to PermissionsProvider
Permissions.Products.Actions.View,
Permissions.Products.Actions.Create,
Permissions.Products.Actions.Update,
Permissions.Products.Actions.Delete
```

### 4. Create Migration
```powershell
# Create and apply migration
dotnet ef migrations add AddProduct
dotnet ef database update
```

### 5. Verify Build
```powershell
# Ensure everything compiles
dotnet build

# Run tests
dotnet test
```

## Advanced Usage

### Custom Templates
```powershell
# Use specific template
qcli add Product --all --template minimal

# Available templates: default, minimal, advanced
```

### Batch Generation
```powershell
# Generate multiple related entities
qcli add Product --all
qcli add Category --all
qcli add ProductCategory --all --entity-type Audited
```

### Output Control
```powershell
# Specify custom output path
qcli add Product --all --output custom/path/

# Verbose output for debugging
qcli add Product --all --verbose
```

## Common Patterns

### Read-Only Entities
```powershell
# Generate only queries for reporting
qcli add SalesReport --read --no-permissions
```

### Reference Data
```powershell
# Simple lookup tables
qcli add Status --all --entity-type Entity --no-tests
```

### Audit-Heavy Entities
```powershell
# Financial or sensitive data
qcli add Transaction --all --entity-type FullyAudited
```

## Troubleshooting

### Build Errors
- Ensure entity is added to DbContext
- Check for missing using statements
- Verify entity configuration is registered

### Permission Errors
- Add generated permissions to PermissionsProvider
- Ensure authorization middleware is configured

### Migration Issues
- Review generated entity configuration
- Check for naming conflicts with existing entities

## Best Practices

1. **Entity Naming**: Use singular names (Product, not Products)
2. **Entity Types**: Use `FullyAudited` for business-critical data
3. **Generation Order**: Generate core entities first
4. **Testing**: Keep generated tests, customize as needed
5. **Customization**: Modify generated code after integration

---

**Next Steps:**
- [Integration Guide](../advanced/integration.md) - Complete integration steps
- [Configuration](../configuration/configuration.md) - Customize generation
- [Best Practices](../advanced/best-practices.md) - Development guidelines
