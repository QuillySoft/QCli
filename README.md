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

## 🚀 Quick Start

### Setup from GitHub

```powershell
# Clone the repository
git clone https://github.com/QuillySoft/QCli.git
cd QCli

# Build and install as global tool
dotnet pack src/Apps/Tools.Cli/Tools.Cli.csproj
dotnet tool install --global --add-source ./src/Apps/Tools.Cli/bin/Debug QuillySOFT.CLI

# Generate your first CRUD entity
qcli add Product --all

# 20+ files generated instantly ✨
```

### Install from NuGet (Coming Soon)

```powershell
# Will be available soon via NuGet
dotnet tool install --global QuillySOFT.CLI
```

## 💡 What You Get

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

## ⚡ Essential Commands

| Command | Description | Example |
|---------|-------------|---------|
| `add` | Generate CRUD operations | `qcli add Order --all` |
| `config` | Manage configuration | `qcli config show` |
| `init` | Initialize project | `qcli init` |
| `doctor` | Diagnose issues | `qcli doctor` |

### Quick Examples

```powershell
# Complete CRUD with audit fields
qcli add Product --all --entity-type Audited

# With soft delete capability
qcli add Order --all --entity-type FullyAudited

# Skip tests and permissions for simple entities
qcli add Category --all --no-tests --no-permissions
```

## 🏗️ Quick Integration

After generation, complete these steps:

1. **Add to DbContext**: `public DbSet<Product> Products { get; set; }`
2. **Register configuration**: `modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());`
3. **Add permissions** to your permissions provider
4. **Create migration**: `dotnet ef migrations add AddProduct && dotnet ef database update`

## 📚 Documentation

### Quick Start Guides
- **[📦 Installation](src/docs/installation.md)** - Setup and installation
- **[⚡ Quick Start](src/docs/quick-start.md)** - Get running in minutes
- **[🛠️ Project Setup](src/docs/project-setup.md)** - Initialize QCLI in your project

### Commands Reference
- **[add Command](src/docs/commands/add.md)** - Generate CRUD operations
- **[config Command](src/docs/commands/config.md)** - Manage configuration settings
- **[init Command](src/docs/commands/init.md)** - Initialize project configuration
- **[doctor Command](src/docs/commands/doctor.md)** - Diagnose and fix issues
- **[list Command](src/docs/commands/list.md)** - List available templates and entities
- **[scaffold Command](src/docs/commands/scaffold.md)** - Scaffold entire project structure
- **[update Command](src/docs/commands/update.md)** - Update templates and tool

### Configuration & Customization
- **[Configuration Guide](src/docs/configuration/configuration.md)** - Complete configuration reference
- **[Project Paths](src/docs/configuration/paths.md)** - Customize file generation paths
- **[Code Generation Settings](src/docs/configuration/code-generation.md)** - Control what gets generated
- **[Templates](src/docs/configuration/templates.md)** - Template system configuration

### Architecture & Patterns
- **[Clean Architecture](src/docs/architecture/clean-architecture.md)** - Architecture overview and implementation
- **[CQRS Pattern](src/docs/architecture/cqrs.md)** - Command Query Responsibility Segregation
- **[Entity Types](src/docs/architecture/entity-types.md)** - Understanding entity inheritance
- **[Permissions System](src/docs/architecture/permissions.md)** - Authorization structure
- **[Validation](src/docs/architecture/validation.md)** - FluentValidation integration
- **[Domain Events](src/docs/architecture/domain-events.md)** - Event-driven architecture
- **[Mapping Profiles](src/docs/architecture/mapping-profiles.md)** - Object-to-object mapping
- **[Testing Patterns](src/docs/architecture/testing-patterns.md)** - Comprehensive testing strategies

### Examples & Use Cases
- **[E-commerce Example](src/docs/examples/ecommerce.md)** - Build an e-commerce system
- **[CRM Example](src/docs/examples/crm.md)** - Customer relationship management
- **[Content Management](src/docs/examples/cms.md)** - Content management system
- **[Enterprise Application](src/docs/examples/enterprise.md)** - Large-scale enterprise app

### Advanced Topics
- **[Customization](src/docs/advanced/customization.md)** - Customize templates and generation
- **[Integration](src/docs/advanced/integration.md)** - Integrate with existing projects
- **[Best Practices](src/docs/advanced/best-practices.md)** - Recommended practices
- **[Troubleshooting](src/docs/advanced/troubleshooting.md)** - Common issues and solutions

### Reference
- **[CLI Reference](src/docs/reference/cli-reference.md)** - Complete command-line reference
- **[Generated Code Structure](src/docs/reference/code-structure.md)** - Understanding generated files
- **[Template Variables](src/docs/reference/template-variables.md)** - Available template variables
- **[Error Codes](src/docs/reference/error-codes.md)** - Error messages and solutions

## 🎯 Real-World Examples

### E-commerce System
```powershell
qcli add Product --all --entity-type Audited
qcli add Order --all --entity-type FullyAudited
qcli add Customer --all --entity-type FullyAudited
```

### CRM Application
```powershell
qcli add Contact --all --entity-type FullyAudited
qcli add Company --all --entity-type Audited
qcli add Opportunity --all --entity-type Audited
```

## 🏗️ Architecture

QCLI generates **Clean Architecture** code with:

- **CQRS** - Separate commands and queries
- **Authorization** - Permission-based security
- **Validation** - FluentValidation with DTOs
- **Testing** - Unit and integration tests
- **Performance** - Optimized queries with pagination

## 📊 Performance Impact

| Task | Manual | QCLI | Savings |
|------|--------|------|---------|
| Complete CRUD entity | 4-5 hours | 30 seconds | **99.86%** |
| Domain + Commands + Tests | 2-3 hours | Instant | **100%** |
| Controller + Integration | 1 hour | Instant | **100%** |

## 🔧 Tech Stack

- **.NET 8** with **C# 12**
- **Entity Framework Core**
- **ASP.NET Core**
- **MediatR** (CQRS)
- **FluentValidation**
- **xUnit** (Testing)

## 🤝 Getting Help

- **[📚 Full Documentation](src/docs/README.md)** - Complete guide and reference
- **[🐛 Troubleshooting](src/docs/advanced/troubleshooting.md)** - Common issues and solutions
- **[❓ FAQ & Support](src/docs/advanced/troubleshooting.md)** - Frequently asked questions

## 📄 License

MIT License - Copyright (c) 2025 QuillySOFT

---

**Stop writing boilerplate. Start building features.** 🚀

**📚 [View Complete Documentation →](src/docs/README.md)**

