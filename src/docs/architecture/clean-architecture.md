# 🏗️ Clean Architecture in QCLI

This guide explains how QCLI implements and generates Clean Architecture patterns, ensuring proper separation of concerns and maintainable enterprise applications.

## 📋 Table of Contents

- [Architecture Overview](#architecture-overview)
- [Layer Responsibilities](#layer-responsibilities)
- [Project Structure](#project-structure)
- [Dependency Flow](#dependency-flow)
- [Generated Code Examples](#generated-code-examples)
- [Best Practices](#best-practices)
- [Common Patterns](#common-patterns)
- [Integration Guidelines](#integration-guidelines)

## Architecture Overview

QCLI generates code following **Clean Architecture** principles with these core concepts:

### 🎯 Core Principles

1. **Dependency Inversion**: Outer layers depend on inner layers, never the reverse
2. **Separation of Concerns**: Each layer has a single, well-defined responsibility
3. **Independent Testability**: Business logic can be tested without external dependencies
4. **Framework Independence**: Business rules don't depend on frameworks or databases
5. **Database Independence**: Business logic doesn't depend on specific database implementations

### 🔄 Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                     │
│                    (Controllers, APIs)                     │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                       │
│               (Commands, Queries, DTOs)                    │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                          │
│                (Entities, Business Rules)                  │
├─────────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                      │
│              (Database, External Services)                 │
└─────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### 🌐 Presentation Layer (`src/Apps/Api`)

**Purpose**: Handle HTTP requests, route to application layer, format responses

**Generated Components:**
- **Controllers**: REST API endpoints with proper HTTP verbs
- **Request/Response Models**: API-specific DTOs
- **Authentication**: JWT token validation
- **Error Handling**: Global exception handling

**Example Generated Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Post(
        ProductForCreateUpdateDto productDto, 
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(
            new CreateProductCommand(productDto), 
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductForReadDto>> GetById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(
            new GetProductByIdQuery(id), 
            cancellationToken));
    }
}
```

### 🎯 Application Layer (`src/Core/Application`)

**Purpose**: Orchestrate business workflows, handle use cases, coordinate domain operations

**Generated Components:**
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List, Search)
- **DTOs**: Data transfer objects for API communication
- **Validators**: Input validation using FluentValidation
- **Event Handlers**: Process domain events
- **Mapping Profiles**: Object mapping configurations

**CQRS Implementation:**
```csharp
// Command Example
[Authorize(Permissions = [Permissions.Products.Actions.Create])]
public sealed class CreateProductCommand(ProductForCreateUpdateDto productDto) 
    : IRequest<Guid>
{
    public ProductForCreateUpdateDto ProductDto { get; } = productDto;

    public sealed class Handler(IApplicationDbContext dbContext) 
        : IRequestHandler<CreateProductCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateProductCommand request, 
            CancellationToken cancellationToken)
        {
            var product = new Product(Guid.NewGuid(), request.ProductDto.Name);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}

// Query Example
[Authorize(Permissions = [Permissions.Products.Actions.View])]
public sealed class GetProductsQuery(PaginatedRequestDto requestDto) 
    : IRequest<PaginatedList<ProductForListDto>>
{
    public PaginatedRequestDto RequestDto { get; } = requestDto;

    public sealed class Handler(IApplicationDbContext dbContext) 
        : IRequestHandler<GetProductsQuery, PaginatedList<ProductForListDto>>
    {
        public async Task<PaginatedList<ProductForListDto>> Handle(
            GetProductsQuery request, 
            CancellationToken cancellationToken)
        {
            var query = dbContext.Products
                .AsNoTracking()
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.RequestDto.Search))
            {
                var search = request.RequestDto.Search.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(search));
            }

            return await query
                .ProjectToListAsync<ProductForListDto>(
                    request.RequestDto, 
                    cancellationToken);
        }
    }
}
```

### 🏛️ Domain Layer (`src/Core/Domain`)

**Purpose**: Core business logic, entities, domain rules, and domain events

**Generated Components:**
- **Entities**: Rich domain models with behavior
- **Value Objects**: Immutable objects representing concepts
- **Domain Events**: Events that occur within the domain
- **Permissions**: Authorization constants
- **Interfaces**: Domain service contracts

**Entity Inheritance Hierarchy:**
```csharp
// Base Entity
public abstract class Entity<T> where T : notnull
{
    public T Id { get; protected set; }
    
    protected Entity(T id) => Id = id;
}

// Audited Entity (Default)
public abstract class AuditedEntity<T> : Entity<T> where T : notnull
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }
    public string UpdatedBy { get; private set; } = string.Empty;

    protected AuditedEntity(T id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

// Fully Audited Entity (with Soft Delete)
public abstract class FullyAuditedEntity<T> : AuditedEntity<T> where T : notnull
{
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    protected FullyAuditedEntity(T id) : base(id) { }

    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}

// Generated Product Entity
public sealed class Product : AuditedEntity<Guid>
{
    public string Name { get; private set; }
    
    private Product(Guid id) : base(id) { }

    public Product(Guid id, string name) : this(id)
    {
        Name = name;
    }

    public void Update(string name)
    {
        Name = name;
    }
}
```

### 🏗️ Infrastructure Layer (`src/Infrastructure/Persistence`)

**Purpose**: External concerns, database access, third-party integrations

**Generated Components:**
- **Entity Configurations**: EF Core entity mappings
- **DbContext**: Database context with DbSets
- **Repositories**: Data access implementations
- **External Services**: Third-party service integrations

**Entity Configuration Example:**
```csharp
public sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Configure audit fields if needed
        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);
    }
}
```

## Project Structure

### 📁 Complete Project Layout

```
src/
├── 🌐 Apps/
│   └── Api/                           # Presentation Layer
│       ├── Controllers/               # REST API controllers
│       │   ├── ProductsController.cs
│       │   ├── OrdersController.cs
│       │   └── CustomersController.cs
│       ├── Program.cs                 # Application entry point
│       └── appsettings.json          # Configuration
│
├── 🎯 Core/
│   ├── Application/                   # Application Layer
│   │   ├── Products/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateProduct/
│   │   │   │   │   ├── CreateProductCommand.cs
│   │   │   │   │   └── CreateProductCommandValidator.cs
│   │   │   │   ├── UpdateProduct/
│   │   │   │   └── DeleteProduct/
│   │   │   ├── Queries/
│   │   │   │   ├── GetProducts/
│   │   │   │   │   └── GetProductsQuery.cs
│   │   │   │   └── GetProductDetails/
│   │   │   │       └── GetProductByIdQuery.cs
│   │   │   ├── Events/                # Domain event handlers
│   │   │   │   ├── ProductCreatedEvent.cs
│   │   │   │   └── ProductUpdatedEvent.cs
│   │   │   └── Mapping/               # Object mapping
│   │   │       └── ProductMappingProfile.cs
│   │   ├── Common/                    # Shared application components
│   │   │   ├── Interfaces/
│   │   │   │   └── IApplicationDbContext.cs
│   │   │   ├── Exceptions/
│   │   │   │   └── NotFoundException.cs
│   │   │   ├── DTOs/
│   │   │   │   ├── ProductForCreateUpdateDto.cs
│   │   │   │   ├── ProductForReadDto.cs
│   │   │   │   └── ProductForListDto.cs
│   │   │   └── PaginatedList.cs
│   │   └── Security/
│   │       └── AuthorizeAttribute.cs
│   │
│   └── Domain/                        # Domain Layer
│       ├── Products/
│       │   └── Product.cs             # Domain entity
│       ├── Orders/
│       │   ├── Order.cs
│       │   └── OrderItem.cs
│       ├── Common/                    # Shared domain components
│       │   ├── Entity.cs
│       │   ├── AuditedEntity.cs
│       │   └── FullyAuditedEntity.cs
│       └── PermissionsConstants/      # Authorization permissions
│           ├── ProductsPermissions.cs
│           └── OrdersPermissions.cs
│
└── 🏗️ Infrastructure/
    └── Persistence/                   # Infrastructure Layer
        ├── Configurations/            # EF Core configurations
        │   ├── Products/
        │   │   └── ProductEntityConfiguration.cs
        │   └── Orders/
        │       ├── OrderEntityConfiguration.cs
        │       └── OrderItemEntityConfiguration.cs
        ├── ApplicationDbContext.cs    # Database context
        └── Migrations/                # EF Core migrations

tests/                                 # Test Projects
├── Application.Tests/                 # Unit tests
│   ├── Products/
│   │   ├── Commands/
│   │   │   └── CreateProductCommandTests.cs
│   │   └── Queries/
│   │       └── GetProductsQueryTests.cs
│   └── Common/
└── Integration.Tests/                 # Integration tests
    └── Controllers/
        └── ProductsControllerTests.cs
```

## Dependency Flow

### 🔄 Dependency Direction

```
Presentation → Application → Domain ← Infrastructure
     ↓              ↓          ↑            ↑
Controllers → Commands/    Entities    DbContext
APIs       → Queries   ← Business  ← Configurations
           → DTOs       Rules     ← Repositories
```

### 📦 Dependency Rules

1. **Domain** has no dependencies (pure business logic)
2. **Application** depends only on Domain
3. **Infrastructure** depends on Domain and Application interfaces
4. **Presentation** depends on Application abstractions

### 🔌 Dependency Injection

QCLI generates code that follows proper DI patterns:

```csharp
// Application layer dependencies
public sealed class CreateProductCommand : IRequest<Guid>
{
    public sealed class Handler(IApplicationDbContext dbContext) 
        : IRequestHandler<CreateProductCommand, Guid>
    {
        // Handler depends on interface, not concrete implementation
    }
}

// Infrastructure implements interface
public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Product> Products { get; set; }
    // Implementation details
}
```

## Generated Code Examples

### 🎯 Complete CRUD Example

When you run `qcli add Product --all`, QCLI generates:

#### Domain Entity
```csharp
public sealed class Product : AuditedEntity<Guid>
{
    public string Name { get; private set; }
    
    private Product(Guid id) : base(id) { }

    public Product(Guid id, string name) : this(id)
    {
        Name = name;
    }

    public void Update(string name)
    {
        Name = name;
    }
}
```

#### Create Command
```csharp
[Authorize(Permissions = [Permissions.Products.Actions.Create])]
public sealed class CreateProductCommand(ProductForCreateUpdateDto productDto) 
    : IRequest<Guid>
{
    public ProductForCreateUpdateDto ProductDto { get; } = productDto;

    public sealed class Handler(IApplicationDbContext dbContext) 
        : IRequestHandler<CreateProductCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateProductCommand request, 
            CancellationToken cancellationToken)
        {
            var product = new Product(Guid.NewGuid(), request.ProductDto.Name);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}
```

#### Query with Pagination
```csharp
[Authorize(Permissions = [Permissions.Products.Actions.View])]
public sealed class GetProductsQuery(PaginatedRequestDto requestDto) 
    : IRequest<PaginatedList<ProductForListDto>>
{
    public PaginatedRequestDto RequestDto { get; } = requestDto;

    public sealed class Handler(IApplicationDbContext dbContext) 
        : IRequestHandler<GetProductsQuery, PaginatedList<ProductForListDto>>
    {
        public async Task<PaginatedList<ProductForListDto>> Handle(
            GetProductsQuery request, 
            CancellationToken cancellationToken)
        {
            var query = dbContext.Products
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.RequestDto.Search))
            {
                var search = request.RequestDto.Search.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(search));
            }

            return await query
                .ProjectToListAsync<ProductForListDto>(
                    request.RequestDto, 
                    cancellationToken);
        }
    }
}
```

#### REST Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Post(
        ProductForCreateUpdateDto productDto, 
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(
            new CreateProductCommand(productDto), 
            cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProductForListDto>>> Get(
        [FromQuery] PaginatedRequestDto requestDto, 
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(
            new GetProductsQuery(requestDto), 
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductForReadDto>> GetById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(
            new GetProductByIdQuery(id), 
            cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Put(
        Guid id, 
        ProductForCreateUpdateDto productDto, 
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProductCommand(id, productDto), 
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(
        Guid id, 
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteProductCommand(id), 
            cancellationToken);
        return NoContent();
    }
}
```

## Best Practices

### 🎯 Domain Layer Best Practices

1. **Rich Domain Models**: Encapsulate business logic in entities
```csharp
public sealed class Order : AuditedEntity<Guid>
{
    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    
    public decimal TotalAmount => _items.Sum(x => x.Amount);
    
    public void AddItem(OrderItem item)
    {
        if (item.Amount <= 0)
            throw new InvalidOperationException("Item amount must be positive");
            
        _items.Add(item);
    }
}
```

2. **Value Objects**: Use for concepts without identity
```csharp
public sealed record Email(string Value)
{
    public Email(string value) : this(value?.Trim().ToLowerInvariant() ?? "")
    {
        if (string.IsNullOrWhiteSpace(Value))
            throw new ArgumentException("Email cannot be empty");
            
        if (!IsValidEmail(Value))
            throw new ArgumentException("Invalid email format");
    }
    
    private static bool IsValidEmail(string email) =>
        new EmailAddressAttribute().IsValid(email);
}
```

### 🎯 Application Layer Best Practices

1. **Command Handlers**: Keep focused on single responsibility
2. **Query Handlers**: Use read-only operations with projection
3. **Validation**: Use FluentValidation for input validation
4. **Error Handling**: Use custom exceptions and global handlers

### 🎯 Infrastructure Best Practices

1. **Entity Configurations**: Keep EF configurations separate
2. **Repository Pattern**: Use when complex queries are needed
3. **Unit of Work**: Leverage EF's built-in unit of work

## Common Patterns

### 🔄 CQRS Pattern

QCLI implements Command Query Responsibility Segregation:

- **Commands**: Modify state, return minimal data
- **Queries**: Read data, never modify state
- **Separation**: Different models for read and write operations

### 🔐 Authorization Pattern

Consistent permission-based authorization:

```csharp
public static partial class Permissions
{
    public static class Products
    {
        public static class Actions
        {
            public const string View = "Permissions:Products:View";
            public const string Profile = "Permissions:Products:Profile";
            public const string Create = "Permissions:Products:Create";
            public const string Update = "Permissions:Products:Update";
            public const string Delete = "Permissions:Products:Delete";
        }
    }
}
```

### 📊 Pagination Pattern

All list queries include pagination support:

```csharp
public async Task<PaginatedList<T>> ProjectToListAsync<T>(
    PaginatedRequestDto request, 
    CancellationToken cancellationToken)
{
    var totalCount = await query.CountAsync(cancellationToken);
    var items = await query
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(projection)
        .ToListAsync(cancellationToken);
        
    return new PaginatedList<T>(items, totalCount, request.PageNumber, request.PageSize);
}
```

## Integration Guidelines

### 🔧 Database Integration

1. **Add DbSet to Context:**
```csharp
public DbSet<Product> Products { get; set; }
```

2. **Register Entity Configuration:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
}
```

3. **Create Migration:**
```bash
dotnet ef migrations add AddProduct
dotnet ef database update
```

### 🔧 Permission Integration

Add permissions to your authorization system:

```csharp
public static readonly string[] AllPermissions = {
    Permissions.Products.Actions.View,
    Permissions.Products.Actions.Create,
    Permissions.Products.Actions.Update,
    Permissions.Products.Actions.Delete,
    // ... other permissions
};
```

### 🔧 Validation Integration

Customize generated validators:

```csharp
public sealed class ProductForCreateUpdateDtoValidator 
    : AbstractValidator<ProductForCreateUpdateDto>
{
    public ProductForCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");
            
        // Add custom business rules
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero");
    }
}
```

---

**Related Documentation:**
- [CQRS Pattern](cqrs.md) - Command Query Responsibility Segregation
- [Entity Types](entity-types.md) - Understanding entity inheritance
- [Permissions](permissions.md) - Authorization structure
- [Configuration Guide](../configuration/configuration.md) - Project configuration
