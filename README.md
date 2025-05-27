# 🚀 QCLI - QuillySOFT CLI

**Generate enterprise-grade CRUD operations in 30 seconds**

QCLI eliminates boilerplate code by generating Clean Architecture CRUD operations following CQRS and DDD patterns.


## ⚡ Why QCLI?

### **Before QCLI: The Manual Pain**
Creating a single CRUD entity traditionally requires:
- 📝 **2-4 hours** of repetitive coding
- 🗂️ **20+ files** with complex patterns
- 🔄 **500+ lines** of boilerplate code
- 🐛 **Error-prone** manual implementation
- 📚 **Remembering** architectural patterns

### **After QCLI: The Magic**
- ⏱️ **30 seconds** to generate complete CRUD
- 🎯 **One command** creates everything
- ✅ **Zero errors** - guaranteed compilation
- 🏗️ **Enterprise patterns** built-in
- 🎨 **Focus on business logic**, not plumbing


## Quick Start

```powershell
# Install
dotnet tool install --global QuillySOFT.CLI

# Generate CRUD entity
qcli add Product --all

# 20+ files generated instantly ✨
```

## What You Get

Generate complete CRUD operations with:
- **Domain entities** with proper inheritance
- **CQRS commands & queries** with validation
- **REST controllers** with authorization
- **Unit & integration tests**
- **EF Core configurations**
- **Permission structures**

```
Product entity generates:
├── Domain/Products/Product.cs
├── Application/Products/Commands/Create|Update|Delete
├── Application/Products/Queries/GetProducts|GetById  
├── Controllers/ProductsController.cs
├── Configurations/ProductEntityConfiguration.cs
├── Tests/Commands & Queries
└── ProductsPermissions.cs
```

## Commands

| Command | Description | Example |
|---------|-------------|---------|
| `add` | Generate CRUD operations | `qcli add Order --all` |
| `config` | Manage configuration | `qcli config show` |
| `init` | Initialize project | `qcli init` |
| `doctor` | Diagnose issues | `qcli doctor` |

### Add Command Options

```powershell
# Complete CRUD
qcli add Product --all

# Specific operations
qcli add Customer --create --read

# Entity types
qcli add Order --all --entity-type FullyAudited  # with soft delete
qcli add Product --all --entity-type Audited     # with audit fields
qcli add Category --all --entity-type Entity     # basic entity

# Skip components
qcli add Simple --all --no-tests --no-permissions
```

## Integration Steps

After generation:

1. **Add to DbContext**
```csharp
public DbSet<Product> Products { get; set; }
```

2. **Register configuration**
```csharp
modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
```

3. **Add permissions**
```csharp
Permissions.Products.Actions.View,
Permissions.Products.Actions.Create,
// ... etc
```

4. **Create migration**
```powershell
dotnet ef migrations add AddProduct
dotnet ef database update
```

## Architecture

QCLI generates **Clean Architecture** code with:

- **CQRS** - Separate commands and queries
- **Authorization** - Permission-based security
- **Validation** - FluentValidation with DTOs
- **Testing** - Unit and integration tests
- **Performance** - Optimized queries with pagination

## Configuration

View current settings:
```powershell
qcli config show
```

Customize paths and generation options in `quillysoft-cli.json`:
```json
{
  "projectType": "CleanArchitecture",
  "paths": {
    "domainPath": "src/Core/Domain",
    "applicationPath": "src/Core/Application"
  },
  "codeGeneration": {
    "generateTests": true,
    "generatePermissions": true
  }
}
```

## Examples

**E-commerce**
```powershell
qcli add Product --all --entity-type Audited
qcli add Order --all --entity-type FullyAudited
qcli add Customer --all --entity-type FullyAudited
```

**CRM**
```powershell
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type Audited
qcli add Opportunity --all --entity-type Audited
```

## Performance

| Task | Manual | QCLI | Savings |
|------|--------|------|---------|
| Complete CRUD entity | 4-5 hours | 30 seconds | **99%** |
| Domain + Commands + Tests | 2-3 hours | Instant | **100%** |
| Controller + Integration | 1 hour | Instant | **100%** |

## Tech Stack

- **.NET 8** with **C# 12**
- **Entity Framework Core**
- **ASP.NET Core**
- **MediatR** (CQRS)
- **FluentValidation**
- **xUnit** (Testing)

## License

MIT License - Copyright (c) 2025 QuillySOFT

---

**Stop writing boilerplate. Start building features.** 🚀
    public sealed class Handler : IRequestHandler<GetProductsQuery, PaginatedList<ProductForListDto>>
    {
        // Implementation with filtering, pagination, and optimization
    }
}
```

### **🛡️ Security & Authorization**
```csharp
// Nested permissions structure
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

### **✅ Validation & Error Handling**
```csharp
public sealed class ProductForCreateUpdateDtoValidator : AppBaseAbstractValidator<ProductForCreateUpdateDto>
{
    public ProductForCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Matches(NameRegex)
            .WithMessage("Name must contain only alphabetic characters.")
            .MaximumLength(64)
            .WithMessage("Name cannot exceed 64 characters.");
    }
}
```

## 🛠️ Commands & Usage

### **🎯 Core Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `add` | Generate CRUD operations | `qcli add Order --all` |
| `config` | Manage configuration | `qcli config show` |

### **📝 Add Command - Complete Reference**

#### **Basic Usage**
```powershell
# Generate complete CRUD for an entity
qcli add Product --all

# Generate specific operations only
qcli add Customer --create --read

# Generate with advanced entity type
qcli add Order --all --entity-type FullyAudited
```

#### **Available Options**

| Option | Description | Example |
|--------|-------------|---------|
| `--all` | Generate all CRUD operations | `add Product --all` |
| `--create` | Generate create operation only | `add Product --create` |
| `--read` | Generate read operations (queries) | `add Product --read` |
| `--update` | Generate update operation only | `add Product --update` |
| `--delete` | Generate delete operation only | `add Product --delete` |
| `--entity-type` | Entity base type (`Audited`, `FullyAudited`) | `add Product --all --entity-type FullyAudited` |
| `--no-tests` | Skip generating unit tests | `add Product --all --no-tests` |
| `--no-permissions` | Skip generating permissions | `add Product --all --no-permissions` |

#### **Entity Types Explained**

| Type | Inherits From | Includes | Use Case |
|------|---------------|----------|----------|
| `Entity` | `Entity<Guid>` | Basic entity with Id | Simple entities |
| `Audited` | `AuditedEntity<Guid>` | + CreatedAt, CreatedBy, UpdatedAt, UpdatedBy | Most business entities |
| `FullyAudited` | `FullyAuditedEntity<Guid>` | + DeletedAt, DeletedBy, IsDeleted | Entities requiring soft delete |

### **⚙️ Configuration Commands**

```powershell
# View current configuration
qcli config show

# View available config subcommands
qcli config

# Available subcommands:
# - init    : Initialize configuration file
# - show    : Show current configuration  
# - set     : Set a configuration value
# - get     : Get a configuration value
# - sample  : Generate sample configuration file
```

## 🎯 Real-World Use Cases

### **🛍️ E-Commerce Application**
```powershell
# Product catalog
qcli add Product --all --entity-type Audited
qcli add Category --all --entity-type Audited

# Order management  
qcli add Order --all --entity-type FullyAudited
qcli add OrderItem --all --entity-type Audited

# Customer management
qcli add Customer --all --entity-type FullyAudited
qcli add Address --all --entity-type Audited
```

### **📊 CRM System**
```powershell
# Contact management
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type FullyAudited

# Sales pipeline
qcli add Lead --all --entity-type Audited
qcli add Opportunity --all --entity-type Audited
qcli add Deal --all --entity-type FullyAudited
```

### **📝 Content Management**
```powershell
# Content entities
qcli add Article --all --entity-type FullyAudited
qcli add Page --all --entity-type Audited
qcli add Media --all --entity-type Audited

# User management
qcli add Author --all --entity-type Audited
qcli add Role --all --entity-type Audited
```

### **🏭 Enterprise Application**
```powershell
# Core business entities
qcli add Project --all --entity-type FullyAudited
qcli add Task --all --entity-type FullyAudited
qcli add TimeEntry --all --entity-type Audited

# Reference data
qcli add Department --all --entity-type Audited
qcli add Location --all --entity-type Audited
```

## 💡 Pro Tips & Best Practices

### **🎯 Naming Conventions**
```powershell
# ✅ Good - Singular entity names
qcli add Product --all
qcli add Customer --all
qcli add OrderItem --all

# ❌ Avoid - Plural names (tool will handle pluralization)
qcli add Products --all    # Will create ProductsController, not ProductController
```

### **📁 Entity Organization**
```powershell
# Group related entities together
# Generate core entities first
qcli add Customer --all --entity-type FullyAudited
qcli add Product --all --entity-type Audited

# Then supporting entities
qcli add Order --all --entity-type FullyAudited
qcli add OrderItem --all --entity-type Audited
```

### **🔧 Development Workflow**
```powershell
# 1. Generate entity structure
qcli add Product --all

# 2. Verify build
dotnet build

# 3. Review generated files and customize business logic
# 4. Add entity to DbContext (as instructed)
# 5. Create and run migration
dotnet ef migrations add AddProductEntity
dotnet ef database update
```

## ⚙️ Configuration & Customization

### **Current Configuration**
View your current settings:
```powershell
qcli config show
```

**Example Output:**
```
┌───────────────────────────┬────────────────────────────────────┐
│ Setting                   │ Value                              │
├───────────────────────────┼────────────────────────────────────┤
│ Project Type              │ CleanArchitecture                  │
│ Root Path                 │ C:/projects/QCLI                   │
│ API Path                  │ src\Apps\Api                       │
│ Application Path          │ src\Core\Application               │
│ Domain Path               │ src\Core\Domain                    │
│ Persistence Path          │ src\Infra\Persistence              │
│ Default Entity Type       │ Audited                            │
│ Generate Events           │ False                              │
│ Generate Mapping Profiles │ False                              │
│ Generate Permissions      │ True                               │
│ Generate Tests            │ True                               │
└───────────────────────────┴────────────────────────────────────┘
```

### **Configuration Options**

| Setting | Default | Description |
|---------|---------|-------------|
| **Generate Events** | `False` | Create domain events for audit logging |
| **Generate Mapping Profiles** | `False` | Create Mapster mapping profiles |
| **Generate Permissions** | `True` | Create enterprise permission structure |
| **Generate Tests** | `True` | Create comprehensive unit tests |
| **Default Entity Type** | `Audited` | Default inheritance for new entities |

### **Project Structure**
QCLI follows Clean Architecture conventions:

```
src/
├── 🌐 Apps/
│   └── Api/                    # Presentation Layer
│       └── Controllers/        # REST API controllers
├── 🎯 Core/
│   ├── Application/            # Application Layer
│   │   └── [Entities]/
│   │       ├── Commands/       # CQRS Commands
│   │       ├── Queries/        # CQRS Queries
│   │       ├── Events/         # Domain Events (optional)
│   │       └── Mapping/        # Mapster Profiles (optional)
│   └── Domain/                 # Domain Layer
│       ├── [Entities]/         # Domain entities
│       └── PermissionsConstants/ # Authorization permissions
└── 🏗️ Infra/
    └── Persistence/            # Infrastructure Layer
        └── Configurations/     # EF Core configurations
tests/
├── Application/                # Unit tests for application layer
└── Infra/                     # Integration tests
    └── Controllers/            # API integration tests
```

## 🏗️ Integration Steps

After generating your entities, complete these integration steps:

### **1. Database Context Integration**

Add the new entity to your DbContext:

```csharp
// In IApplicationDbContext.cs and ApplicationDbContext.cs
public DbSet<Product> Products { get; set; }
```

### **2. Entity Configuration Registration**

Register the entity configuration:

```csharp
// In ApplicationDbContext.OnModelCreating method
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ... existing configurations
    modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
}
```

### **3. Permissions Registration**

Add permissions to your permissions provider:

```csharp
// In PermissionsProvider.cs
public static class PermissionsProvider
{
    public static readonly string[] AllPermissions = {
        // ... existing permissions
        Permissions.Products.Actions.View,
        Permissions.Products.Actions.Profile,
        Permissions.Products.Actions.Create,
        Permissions.Products.Actions.Update,
        Permissions.Products.Actions.Delete,
    };
}
```

### **4. Database Migration**

Create and apply the database migration:

```powershell
# Create migration
dotnet ef migrations add AddProductEntity

# Apply to database
dotnet ef database update
```

### **5. Verify Build & Tests**

```powershell
# Ensure everything compiles
dotnet build

# Run tests to verify functionality
dotnet test
```

## 🧪 Generated Code Quality

### **✅ What Makes QCLI-Generated Code Production-Ready**

#### **🏗️ Enterprise Architecture Patterns**
- **Clean Architecture** with proper layer separation
- **CQRS** for command/query responsibility segregation
- **Domain-Driven Design** with rich domain entities
- **Dependency Inversion** with proper abstractions

#### **🛡️ Security & Authorization**
- **Permission-based authorization** on all operations
- **Nested permission structure** for enterprise scalability
- **Authorization attributes** on all commands and queries
- **Input validation** with FluentValidation

#### **⚡ Performance Optimizations**
- **AsNoTracking()** for read-only queries
- **Projection to DTOs** to minimize data transfer
- **Pagination support** for large datasets
- **Optimized query patterns** for better performance

#### **🧪 Comprehensive Testing**
- **Unit tests** for all commands and queries
- **Integration tests** for API controllers
- **Validation tests** for DTOs and commands
- **Mock patterns** for clean test isolation

#### **📝 Code Quality Standards**
- **Consistent naming conventions** across all files
- **XML documentation** for all public APIs
- **Proper error handling** with custom exceptions
- **Immutable DTOs** with proper validation rules

### **🔍 Code Example: Generated Command**

```csharp
[Authorize(Permissions = [Permissions.Products.Actions.Create])]
public sealed class CreateProductCommand(ProductForCreateUpdateDto productDto) : IRequest<Guid>
{
    public ProductForCreateUpdateDto ProductDto { get; } = productDto;

    public sealed class Handler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand, Guid>
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

**Key Features:**
- ✅ Authorization with enterprise permissions
- ✅ Proper dependency injection with primary constructors
- ✅ Command pattern with MediatR
- ✅ Async/await for scalability
- ✅ Clean separation of concerns

## 📊 Performance Benefits

### **Development Speed Comparison**

| Task | Manual Development | QCLI Generated | Time Savings |
|------|-------------------|----------------|--------------|
| Domain Entity | 15 minutes | Instant | **100%** |
| Create Command + Validator | 30 minutes | Instant | **100%** |
| Update Command + Validator | 25 minutes | Instant | **100%** |
| Delete Command | 15 minutes | Instant | **100%** |
| Get Queries + DTOs | 45 minutes | Instant | **100%** |
| REST Controller | 30 minutes | Instant | **100%** |
| Permissions Structure | 20 minutes | Instant | **100%** |
| Unit Tests | 60 minutes | Instant | **100%** |
| Integration Tests | 30 minutes | Instant | **100%** |
| Entity Configuration | 10 minutes | Instant | **100%** |
| **TOTAL** | **4-5 hours** | **30 seconds** | **99.86%** |

### **Quality Benefits**

| Aspect | Manual Development | QCLI Generated |
|--------|-------------------|----------------|
| **Consistency** | Varies by developer | 100% consistent patterns |
| **Error Rate** | High (typos, missing patterns) | Zero compilation errors |
| **Architecture Compliance** | Depends on knowledge | 100% compliant |
| **Test Coverage** | Often incomplete | Complete unit + integration |
| **Documentation** | Often missing | XML docs generated |
| **Security** | May miss authorization | All endpoints secured |

## 🔧 Advanced Features

### **🎛️ Configurable Generation**

QCLI supports configurable generation based on your needs:

```powershell
# Generate with specific features
qcli add Product --all --entity-type FullyAudited

# Skip certain components
qcli add SimpleEntity --all --no-tests --no-permissions

# Generate only specific operations
qcli add ReadOnlyEntity --read  # Only queries, no commands
```

### **🧩 Extensible Templates**

The tool uses a template-based approach for easy customization:

```
src/Apps/Tools.Cli/Source/
├── Client.cs.txt                      # Domain entity template
├── CreateClientCommand.cs.txt         # Create command template  
├── UpdateClientCommand.cs.txt         # Update command template
├── GetClientsQuery.cs.txt             # Query templates
├── ClientsController.cs.txt           # Controller template
├── ClientsPermissions.cs.txt          # Permissions template
└── ClientForCreateUpdateDtoValidator.cs.txt # Validation template
```

### **🔍 Dry Run Mode**

Preview what will be generated without creating files:

```powershell
# See what would be generated
qcli add Product --all --dry-run
```

### **📦 Batch Generation**

Generate multiple related entities efficiently:

```powershell
# E-commerce entities
qcli add Product --all
qcli add Category --all  
qcli add Order --all --entity-type FullyAudited
qcli add OrderItem --all

# CRM entities  
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type FullyAudited
qcli add Opportunity --all
```

## 🚀 Technology Stack

### **Core Technologies**
- **.NET 8** - Latest .NET platform
- **C# 12** - Modern language features
- **Entity Framework Core** - Data access
- **ASP.NET Core** - Web API framework
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Mapster** - Object mapping (optional)
- **xUnit** - Unit testing framework

### **Architectural Patterns**
- **Clean Architecture** - Dependency inversion and separation of concerns
- **CQRS** - Command Query Responsibility Segregation
- **Domain-Driven Design** - Rich domain models
- **Repository Pattern** - Data access abstraction
- **Specification Pattern** - Complex query logic
- **Unit of Work** - Transaction management

### **Code Quality Tools**
- **EditorConfig** - Code style consistency
- **Analyzers** - Static code analysis
- **XML Documentation** - API documentation
- **Nullable Reference Types** - Compile-time null safety

## 🛠️ Troubleshooting

### **Common Issues & Solutions**

#### **Build Errors After Generation**
```powershell
# Check for missing using statements
dotnet build

# If compilation errors, ensure all dependencies are installed
dotnet restore
```

#### **Entity Not Found Errors**
```csharp
// Ensure DbSet is added to DbContext
public DbSet<YourEntity> YourEntities { get; set; }
```

#### **Permission Errors**
```csharp
// Ensure permissions are registered in PermissionsProvider
Permissions.YourEntity.Actions.Create,
Permissions.YourEntity.Actions.Update,
// etc...
```

#### **Migration Issues**
```powershell
# If migration fails, check entity configuration
dotnet ef migrations add AddYourEntity --verbose

# Review generated migration file before applying
dotnet ef database update
```

### **Getting Help**

```powershell
# View command help
qcli add --help

# View configuration help  
qcli config --help

# Check tool version and status
qcli --version
```

## 🎯 Best Practices

### **🏗️ Development Workflow**

1. **Plan Your Entities**
   - Identify entity relationships
   - Choose appropriate base types
   - Consider audit requirements

2. **Generate Core Entities First**
   ```powershell
   # Start with foundational entities
   qcli add Customer --all --entity-type FullyAudited
   qcli add Product --all --entity-type Audited
   ```

3. **Integrate Incrementally**
   - Generate one entity at a time
   - Complete integration steps
   - Test before proceeding

4. **Customize Business Logic**
   - Add entity properties
   - Implement business rules
   - Extend validation rules

### **🎨 Code Organization**

```powershell
# Group related entities logically
# Core business entities
qcli add Customer --all --entity-type FullyAudited
qcli add Order --all --entity-type FullyAudited

# Supporting entities  
qcli add Address --all --entity-type Audited
qcli add OrderItem --all --entity-type Audited

# Reference data
qcli add Category --all --entity-type Audited
qcli add Status --all --entity-type Audited
```

### **📊 Performance Considerations**

- **Use appropriate entity types**: `FullyAudited` only when soft delete is needed
- **Optimize queries**: Generated queries include `AsNoTracking()` for reads
- **Leverage pagination**: All list queries include pagination support
- **Consider caching**: Add caching to frequently accessed data

## 🤝 Contributing & Support

### **Development Setup**
```powershell
# Clone repository
git clone <repository-url>
cd QCLI

# Build and test
dotnet build
dotnet test

# Run the tool
cd src/Apps/Tools.Cli
qcli add TestEntity --all
```

### **Extending QCLI**

The tool is designed for extensibility:

1. **Custom Templates**: Modify templates in `src/Apps/Tools.Cli/Source/`
2. **New Generators**: Add new generators in `src/Apps/Tools.Cli/Commands/`  
3. **Configuration Options**: Extend configuration in `Configuration/`

### **Feedback & Issues**

- Report issues through your organization's internal channels
- Suggest improvements and new features
- Share use cases and success stories

## 📄 License & Copyright

**MIT License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2025 QuillySOFT

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

---

## 🎉 Get Started Today!

Ready to boost your productivity by 100X? Start generating enterprise-grade CRUD operations in seconds:

```powershell
# Your first entity in 30 seconds
qcli add Product --all

# Watch the magic happen! ✨
```

**Stop writing boilerplate. Start building features.** 🚀
