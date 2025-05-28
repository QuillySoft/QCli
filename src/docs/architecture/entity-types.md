# 🏛️ Entity Types in QCLI

This guide explains the entity inheritance hierarchy used by QCLI, helping you choose the right base entity type for your domain models.

## 📋 Table of Contents

- [Entity Hierarchy Overview](#entity-hierarchy-overview)
- [Entity Types](#entity-types)
- [Choosing Entity Types](#choosing-entity-types)
- [Generated Code Examples](#generated-code-examples)
- [Customization Options](#customization-options)
- [Best Practices](#best-practices)
- [Migration Strategies](#migration-strategies)

## Entity Hierarchy Overview

QCLI provides a structured entity inheritance hierarchy that supports different levels of audit tracking and functionality:

```
Entity<T>                    # Base entity with just ID
    ↓
AuditedEntity<T>            # + Created/Updated tracking
    ↓
FullyAuditedEntity<T>       # + Soft delete capability
```

### 🎯 Key Benefits

- **Consistent Structure**: All entities follow the same patterns
- **Audit Trails**: Automatic tracking of who and when
- **Soft Delete**: Safe deletion with recovery capabilities
- **Type Safety**: Generic ID types with compile-time validation
- **Performance**: Optimized for Entity Framework Core

## Entity Types

### 🔹 Entity (Basic)

**Use Case**: Simple entities with minimal requirements

**Properties:**
- `Id` (Generic type, usually `Guid`)

**When to Use:**
- Lookup tables
- Reference data
- Simple entities without audit requirements
- Performance-critical scenarios

**Generated Example:**
```csharp
public sealed class Category : Entity<Guid>
{
    public string Name { get; private set; }
    
    private Category(Guid id) : base(id) { }

    public Category(Guid id, string name) : this(id)
    {
        Name = name;
    }

    public void Update(string name)
    {
        Name = name;
    }
}
```

**Command Usage:**
```bash
qcli add Category --all --entity-type Entity
```

### 🔹 Audited (Default)

**Use Case**: Standard business entities requiring audit trails

**Properties:**
- `Id` (Generic type, usually `Guid`)
- `CreatedAt` (`DateTime`)
- `CreatedBy` (`string`)
- `UpdatedAt` (`DateTime`)
- `UpdatedBy` (`string`)

**When to Use:**
- Most business entities
- Entities requiring change tracking
- Compliance and audit requirements
- Standard CRUD operations

**Generated Example:**
```csharp
public sealed class Product : AuditedEntity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    
    private Product(Guid id) : base(id) { }

    public Product(Guid id, string name, decimal price) : this(id)
    {
        Name = name;
        Price = price;
    }

    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
        // UpdatedAt and UpdatedBy are automatically set by the base class
    }
}
```

**Command Usage:**
```bash
qcli add Product --all --entity-type Audited
# OR (Audited is default)
qcli add Product --all
```

### 🔹 FullyAudited (Soft Delete)

**Use Case**: Critical entities requiring complete audit trails and soft delete

**Properties:**
- All `AuditedEntity` properties plus:
- `DeletedAt` (`DateTime?`)
- `DeletedBy` (`string?`)
- `IsDeleted` (`bool`)

**When to Use:**
- Critical business data
- Entities that should never be permanently deleted
- Regulatory compliance requirements
- Data recovery scenarios

**Generated Example:**
```csharp
public sealed class Customer : FullyAuditedEntity<Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    
    private Customer(Guid id) : base(id) { }

    public Customer(Guid id, string name, string email) : this(id)
    {
        Name = name;
        Email = email;
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }

    // Soft delete is handled by the base class
    public void Delete(string deletedBy)
    {
        SoftDelete(deletedBy);
    }
}
```

**Command Usage:**
```bash
qcli add Customer --all --entity-type FullyAudited
```

## Choosing Entity Types

### 🎯 Decision Matrix

| Scenario | Entity Type | Reason |
|----------|-------------|---------|
| **Product Catalog** | `Audited` | Need creation/update tracking |
| **Customer Data** | `FullyAudited` | Critical data, never truly delete |
| **Order Records** | `FullyAudited` | Legal/compliance requirements |
| **Categories/Tags** | `Entity` | Simple reference data |
| **User Preferences** | `Audited` | Track when settings changed |
| **Financial Records** | `FullyAudited` | Regulatory compliance |
| **Configuration Data** | `Entity` | Simple key-value pairs |
| **Audit Logs** | `Entity` | Already audit data itself |

### 📊 Use Case Examples

#### E-commerce Application
```bash
# Critical business entities
qcli add Customer --all --entity-type FullyAudited
qcli add Order --all --entity-type FullyAudited

# Standard business entities  
qcli add Product --all --entity-type Audited
qcli add CartItem --all --entity-type Audited

# Reference data
qcli add Category --all --entity-type Entity
qcli add Tag --all --entity-type Entity
```

#### CRM System
```bash
# Critical customer data
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type FullyAudited
qcli add Deal --all --entity-type FullyAudited

# Business activities
qcli add Activity --all --entity-type Audited
qcli add Note --all --entity-type Audited

# Reference data
qcli add ActivityType --all --entity-type Entity
qcli add Industry --all --entity-type Entity
```

#### Content Management System
```bash
# Content entities
qcli add Article --all --entity-type FullyAudited
qcli add Page --all --entity-type FullyAudited

# Supporting entities
qcli add Comment --all --entity-type Audited
qcli add Media --all --entity-type Audited

# Configuration
qcli add Setting --all --entity-type Entity
qcli add Theme --all --entity-type Entity
```

## Generated Code Examples

### 🔍 Base Entity Classes

#### Entity<T>
```csharp
public abstract class Entity<T> where T : notnull
{
    public T Id { get; protected set; }
    
    protected Entity(T id)
    {
        Id = id;
    }
    
    // Equality comparison based on ID
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<T> other)
            return false;
            
        if (ReferenceEquals(this, other))
            return true;
            
        return Id.Equals(other.Id);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        return left?.Equals(right) ?? right is null;
    }
    
    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}
```

#### AuditedEntity<T>
```csharp
public abstract class AuditedEntity<T> : Entity<T> where T : notnull
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }
    public string UpdatedBy { get; private set; } = string.Empty;

    protected AuditedEntity(T id) : base(id)
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }
    
    protected AuditedEntity(T id, string createdBy) : this(id)
    {
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }
    
    public void SetAuditFields(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
```

#### FullyAuditedEntity<T>
```csharp
public abstract class FullyAuditedEntity<T> : AuditedEntity<T> where T : notnull
{
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    protected FullyAuditedEntity(T id) : base(id) { }
    
    protected FullyAuditedEntity(T id, string createdBy) : base(id, createdBy) { }

    public void SoftDelete(string deletedBy)
    {
        if (IsDeleted) return;
        
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
```

### 🔍 Entity Configuration Examples

#### Basic Entity Configuration
```csharp
public sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
```

#### Audited Entity Configuration
```csharp
public sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Price)
            .HasPrecision(18, 2);
        
        // Audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.UpdatedAt)
            .IsRequired();
            
        builder.Property(x => x.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100);
    }
}
```

#### Fully Audited Entity Configuration
```csharp
public sealed class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        // Audit fields (inherited)
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.UpdatedBy).IsRequired().HasMaxLength(100);
        
        // Soft delete fields
        builder.Property(x => x.DeletedAt);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        
        // Global query filter to exclude soft-deleted records
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
```

## Customization Options

### 🎨 Custom Entity Base Classes

You can create your own entity base classes:

```csharp
// Custom entity with tenant support
public abstract class TenantEntity<T> : AuditedEntity<T> where T : notnull
{
    public Guid TenantId { get; private set; }
    
    protected TenantEntity(T id, Guid tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}

// Custom entity with version tracking
public abstract class VersionedEntity<T> : AuditedEntity<T> where T : notnull
{
    public int Version { get; private set; }
    
    protected VersionedEntity(T id) : base(id)
    {
        Version = 1;
    }
    
    public void IncrementVersion()
    {
        Version++;
    }
}
```

### 🎨 Entity Property Extensions

Add common properties to your entities:

```csharp
public sealed class Product : AuditedEntity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;
    
    // Value objects
    public ProductCode Code { get; private set; }
    public Money Price { get; private set; }
    
    // Navigation properties
    public Category Category { get; private set; }
    public Guid CategoryId { get; private set; }
    
    private readonly List<ProductTag> _tags = [];
    public IReadOnlyList<ProductTag> Tags => _tags.AsReadOnly();
    
    private Product(Guid id) : base(id) { }

    public Product(Guid id, string name, ProductCode code, Money price, Guid categoryId) 
        : this(id)
    {
        Name = name;
        Code = code;
        Price = price;
        CategoryId = categoryId;
    }
    
    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    
    public void ChangePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new InvalidOperationException("Price must be positive");
            
        Price = newPrice;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    
    public void AddTag(ProductTag tag)
    {
        if (_tags.Any(t => t.TagId == tag.TagId))
            return;
            
        _tags.Add(tag);
    }
}
```

## Best Practices

### 🎯 Entity Design Principles

1. **Encapsulation**: Keep entity state private, expose behavior through methods
```csharp
// ✅ Good: Encapsulated behavior
public void ChangePrice(decimal newPrice)
{
    if (newPrice <= 0)
        throw new InvalidOperationException("Price must be positive");
    Price = newPrice;
}

// ❌ Bad: Public setters
public decimal Price { get; set; }
```

2. **Rich Domain Models**: Include business logic in entities
```csharp
public sealed class Order : FullyAuditedEntity<Guid>
{
    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    
    public decimal TotalAmount => _items.Sum(x => x.Amount);
    public bool CanBeShipped => Status == OrderStatus.Confirmed && _items.Any();
    
    public void AddItem(Product product, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");
            
        var existingItem = _items.FirstOrDefault(x => x.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new OrderItem(Guid.NewGuid(), product.Id, quantity, unitPrice));
        }
    }
    
    public void Ship()
    {
        if (!CanBeShipped)
            throw new InvalidOperationException("Order cannot be shipped");
            
        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }
}
```

3. **Constructor Patterns**: Use factory methods and private constructors
```csharp
public sealed class Product : AuditedEntity<Guid>
{
    // Private constructor for EF Core
    private Product(Guid id) : base(id) { }
    
    // Public constructor with required parameters
    public Product(Guid id, string name, decimal price) : this(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Price = price > 0 ? price : throw new ArgumentException("Price must be positive");
    }
    
    // Factory method for complex creation
    public static Product Create(string name, decimal price, Category category)
    {
        var product = new Product(Guid.NewGuid(), name, price);
        product.CategoryId = category.Id;
        product.Category = category;
        return product;
    }
}
```

### 🎯 Audit Field Management

1. **Automatic Audit Updates**: Use EF interceptors or override SaveChanges
```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<AuditedEntity<Guid>>();
    
    foreach (var entry in entries)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entry.Entity.SetAuditFields(_currentUserService.UserId);
                break;
                
            case EntityState.Modified:
                entry.Entity.SetAuditFields(_currentUserService.UserId);
                break;
        }
    }
    
    return await base.SaveChangesAsync(cancellationToken);
}
```

2. **Soft Delete Implementation**: Use query filters
```csharp
// In DbContext.OnModelCreating
foreach (var entityType in modelBuilder.Model.GetEntityTypes())
{
    if (typeof(FullyAuditedEntity<Guid>).IsAssignableFrom(entityType.ClrType))
    {
        modelBuilder.Entity(entityType.ClrType)
            .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
    }
}

private LambdaExpression CreateSoftDeleteFilter(Type entityType)
{
    var parameter = Expression.Parameter(entityType, "e");
    var property = Expression.Property(parameter, nameof(FullyAuditedEntity<Guid>.IsDeleted));
    var condition = Expression.Equal(property, Expression.Constant(false));
    
    return Expression.Lambda(condition, parameter);
}
```

### 🎯 Performance Considerations

1. **Lazy Loading**: Be careful with navigation properties
2. **Projections**: Use DTOs for read operations
3. **Indexing**: Add indexes on audit and query fields
4. **Query Filters**: Ensure global filters don't impact performance

```csharp
// Entity Configuration with performance optimizations
public void Configure(EntityTypeBuilder<Product> builder)
{
    // Primary key
    builder.HasKey(x => x.Id);
    
    // Indexes for common queries
    builder.HasIndex(x => x.CreatedAt);
    builder.HasIndex(x => x.IsDeleted);
    builder.HasIndex(x => new { x.CategoryId, x.IsActive });
    
    // Query filter for soft delete
    builder.HasQueryFilter(x => !x.IsDeleted);
}
```

## Migration Strategies

### 🔄 Changing Entity Types

#### From Entity to AuditedEntity
```csharp
public partial class AddAuditFieldsToCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Categories",
            type: "datetime2",
            nullable: false,
            defaultValue: DateTime.UtcNow);

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Categories", 
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "System");

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Categories",
            type: "datetime2", 
            nullable: false,
            defaultValue: DateTime.UtcNow);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "Categories",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "System");
    }
}
```

#### From AuditedEntity to FullyAuditedEntity
```csharp
public partial class AddSoftDeleteToProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "DeletedAt",
            table: "Products",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            table: "Products",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Products",
            type: "bit",
            nullable: false,
            defaultValue: false);

        // Add index for query filter performance
        migrationBuilder.CreateIndex(
            name: "IX_Products_IsDeleted",
            table: "Products",
            column: "IsDeleted");
    }
}
```

### 🔄 Data Migration Scripts

```sql
-- Update existing records with audit information
UPDATE Products 
SET CreatedAt = GETUTCDATE(),
    CreatedBy = 'DataMigration',
    UpdatedAt = GETUTCDATE(),
    UpdatedBy = 'DataMigration'
WHERE CreatedAt IS NULL;

-- Ensure soft delete fields are properly initialized
UPDATE Products 
SET IsDeleted = 0
WHERE IsDeleted IS NULL;
```

---

**Related Documentation:**
- [Clean Architecture](clean-architecture.md) - Overall architecture patterns
- [CQRS](cqrs.md) - Command Query Responsibility Segregation
- [Permissions](permissions.md) - Authorization structure
- [Configuration Guide](../configuration/configuration.md) - Project configuration
