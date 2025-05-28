# 🚀 Quick Start Guide

Get up and running with QCLI in under 5 minutes. This guide walks you through generating your first CRUD entity.

## Step 1: Initialize Project

```powershell
# Create a new directory for your project
mkdir MyCleanArchProject
cd MyCleanArchProject

# Initialize QCLI configuration
qcli init --template clean-architecture
```

**Output:**
```
🏗️ Initializing QCLI project...
✅ QCLI initialized successfully!
Configuration saved to: C:\MyCleanArchProject\qcli.json
```

## Step 2: Verify Configuration

```powershell
# Check your configuration
qcli config show
```

**Output:**
```
┌────────────────────┬──────────────────────────────┐
│ Setting            │ Value                        │
├────────────────────┼──────────────────────────────┤
│ Project Type       │ CleanArchitecture            │
│ Root Path          │ C:\MyCleanArchProject        │
│ Domain Path        │ src\Core\Domain              │
│ Application Path   │ src\Core\Application         │
│ API Path           │ src\Apps\Api                 │
│ Generate Tests     │ True                         │
│ Generate Permissions│ True                        │
└────────────────────┴──────────────────────────────┘
```

## Step 3: Generate Your First Entity

```powershell
# Generate a complete CRUD entity
qcli add Product --all
```

**Output:**
```
🎯 Generating CRUD operations for Product...

✅ Generated files:
📁 Domain Layer (2 files)
  ├── src/Core/Domain/Products/Product.cs
  └── src/Core/Domain/Products/ProductsPermissions.cs

📁 Application Layer (10 files)
  ├── src/Core/Application/Products/Commands/CreateProduct/
  │   ├── CreateProductCommand.cs
  │   ├── CreateProductCommandValidator.cs
  │   └── ProductForCreateUpdateDto.cs
  ├── src/Core/Application/Products/Commands/UpdateProduct/
  │   ├── UpdateProductCommand.cs
  │   └── UpdateProductCommandValidator.cs
  ├── src/Core/Application/Products/Commands/DeleteProduct/
  │   └── DeleteProductCommand.cs
  └── src/Core/Application/Products/Queries/
      ├── GetProducts/
      │   ├── GetProductsQuery.cs
      │   └── ProductForListDto.cs
      └── GetProductDetails/
          ├── GetProductByIdQuery.cs
          └── ProductForReadDto.cs

📁 Infrastructure Layer (1 file)
  └── src/Infra/Persistence/Configurations/ProductEntityConfiguration.cs

📁 Presentation Layer (1 file)
  └── src/Apps/Api/Controllers/ProductsController.cs

📁 Tests (6 files)
  ├── tests/Application/Products/Commands/CreateProductCommandTests.cs
  ├── tests/Application/Products/Commands/UpdateProductCommandTests.cs
  ├── tests/Application/Products/Commands/DeleteProductCommandTests.cs
  ├── tests/Application/Products/Queries/GetProductsQueryTests.cs
  ├── tests/Application/Products/Queries/GetProductByIdQueryTests.cs
  └── tests/Infra/Controllers/ProductsControllerTests.cs

🎉 Successfully generated 20 files for Product entity!

📋 Next steps:
1. Add DbSet<Product> Products { get; set; } to your DbContext
2. Register ProductEntityConfiguration in OnModelCreating
3. Add ProductsPermissions to your PermissionsProvider
4. Create and run migration: dotnet ef migrations add AddProduct
```

## Step 4: Examine Generated Files

### Domain Entity
```csharp
// src/Core/Domain/Products/Product.cs
public sealed class Product : AuditedEntity<Guid>
{
    public Product(Guid id, string name) : base(id)
    {
        Name = name;
    }
    
    public string Name { get; private set; } = string.Empty;
    
    public void UpdateName(string name)
    {
        Name = name;
    }
}
```

### Create Command
```csharp
// src/Core/Application/Products/Commands/CreateProduct/CreateProductCommand.cs
[Authorize(Permissions = [Permissions.Products.Actions.Create])]
public sealed class CreateProductCommand(ProductForCreateUpdateDto productDto) : IRequest<Guid>
{
    public ProductForCreateUpdateDto ProductDto { get; } = productDto;

    public sealed class Handler(IApplicationDbContext dbContext) 
        : IRequestHandler<CreateProductCommand, Guid>
    {
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(Guid.NewGuid(), request.ProductDto.Name);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}
```

### REST Controller
```csharp
// src/Apps/Api/Controllers/ProductsController.cs
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct([FromBody] ProductForCreateUpdateDto productDto)
    {
        var command = new CreateProductCommand(productDto);
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = result }, result);
    }
    
    // ... other endpoints
}
```

## Step 5: Integration Steps

Now integrate the generated code:

### 1. Add to DbContext
```csharp
// In your IApplicationDbContext.cs
public DbSet<Product> Products { get; set; }

// In your ApplicationDbContext.cs
public DbSet<Product> Products { get; set; }
```

### 2. Register Entity Configuration
```csharp
// In ApplicationDbContext.OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ... existing configurations
    modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
}
```

### 3. Add Permissions
```csharp
// In your PermissionsProvider.cs
public static readonly string[] AllPermissions = {
    // ... existing permissions
    Permissions.Products.Actions.View,
    Permissions.Products.Actions.Create,
    Permissions.Products.Actions.Update,
    Permissions.Products.Actions.Delete,
};
```

### 4. Create Migration
```powershell
# Create database migration
dotnet ef migrations add AddProduct

# Apply to database
dotnet ef database update
```

### 5. Build and Test
```powershell
# Verify everything compiles
dotnet build

# Run tests
dotnet test
```

## What You Just Created

In less than 5 minutes, you generated:

✅ **20 production-ready files**  
✅ **Complete CRUD operations** with validation  
✅ **REST API endpoints** with Swagger documentation  
✅ **Authorization** with permission-based security  
✅ **Unit & integration tests** with full coverage  
✅ **Clean Architecture** structure  
✅ **CQRS pattern** implementation  

## Next Steps

### Generate More Entities
```powershell
# E-commerce entities
qcli add Category --all --entity-type Audited
qcli add Order --all --entity-type FullyAudited
qcli add Customer --all --entity-type FullyAudited

# Check what you can generate
qcli add --help
```

### Explore Entity Types
```powershell
# Basic entity (just Id)
qcli add SimpleEntity --all --entity-type Entity

# Audited entity (CreatedAt, UpdatedAt, etc.)
qcli add AuditedEntity --all --entity-type Audited

# Fully audited (includes soft delete)
qcli add FullEntity --all --entity-type FullyAudited
```

### Customize Generation
```powershell
# Generate specific operations only
qcli add ReadOnlyEntity --read  # Only queries
qcli add WriteOnlyEntity --create --update --delete  # No read operations

# Skip components
qcli add SimpleEntity --all --no-tests --no-permissions
```

### Preview Before Generation
```powershell
# See what would be generated without creating files
qcli add TestEntity --all --dry-run
```

## Tips for Success

💡 **Start Simple**: Begin with basic entities, then add complexity  
💡 **Use Consistent Naming**: Singular entity names work best  
💡 **Review Generated Code**: Understand the patterns before customizing  
💡 **Test Integration**: Build and test after each entity  
💡 **Explore Options**: Use `--help` on any command for details  

## Common Patterns

### E-commerce Quick Setup
```powershell
qcli add Product --all --entity-type Audited
qcli add Category --all --entity-type Audited
qcli add Order --all --entity-type FullyAudited
qcli add Customer --all --entity-type FullyAudited
```

### CRM Quick Setup
```powershell
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type Audited
qcli add Opportunity --all --entity-type Audited
qcli add Deal --all --entity-type FullyAudited
```

---

**Ready to dive deeper?**
- [Commands Reference](commands/add.md) - Detailed command documentation
- [Configuration Guide](configuration/configuration.md) - Customize QCLI behavior
- [Examples](examples/ecommerce.md) - Real-world use cases
