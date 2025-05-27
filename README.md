# QuillySOFT CLI (QCLI)

A powerful CLI tool for generating CRUD operations in CLIO (Clean, Layered, Integrated, Organized) architecture projects.

## 🚀 Quick Start

```bash
# 1. Build and install
git clone https://github.com/QuillySoft/QCli
cd QCli/src/Apps/Tools.Cli
dotnet pack -c Release
dotnet tool install --global --add-source ./bin/Release QuillySOFT.CLI

# 2. Initialize in your project
cd /path/to/your/clio/project
qcli init

# 3. Generate your first entity
qcli add Order --all
```

## 📋 Commands

| Command | Description | Example |
|---------|-------------|---------|
| `init` | Initialize QCLI in project | `qcli init --interactive` |
| `add` | Generate CRUD operations | `qcli add Order --all --dry-run` |
| `config` | Manage configuration | `qcli config show` |
| `doctor` | Diagnose issues | `qcli doctor --fix` |
| `scaffold` | Create new project | `qcli scaffold MyApi` |

### Add Command Options
- `--all` - Generate complete CRUD (create, read, update, delete)
- `--create`, `--read`, `--update`, `--delete` - Generate specific operations
- `--entity-type` - Specify Audited or FullyAudited
- `--dry-run` - Preview without creating files
- `--no-tests`, `--no-permissions` - Skip generating tests/permissions

## 🔧 What Gets Generated

When you run `qcli add Order --all`:

```
├── Domain/Orders/Order.cs (entity + permissions)
├── Application/Orders/
│   ├── Commands/ (Create, Update, Delete + handlers)
│   └── Queries/ (GetAll, GetById + handlers)
├── Infrastructure/OrderConfiguration.cs
├── WebApi/OrdersController.cs
└── Tests/ (unit + integration tests)
```

---
## ⚙️ Configuration

The tool uses `qcli.json` for project-specific configuration:

```json
{
  "projectType": "CLIO",
  "paths": {
    "rootPath": "c:\\projects\\MyProject",
    "apiPath": "src\\Apps\\Api",
    "applicationPath": "src\\Core\\Application",
    "domainPath": "src\\Core\\Domain",
    "persistencePath": "src\\Infra\\Persistence",
    "applicationTestsPath": "tests\\Application",
    "integrationTestsPath": "tests\\Integration",
    "controllersPath": "src\\Apps\\Api\\Controllers"
  },
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": false,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

## 🏗️ Manual Steps After Generation

After running the tool, complete these steps:

1. **Add DbSet to DbContext:**
   ```csharp
   public DbSet<Order> Orders { get; set; }
   ```

2. **Register Entity Configuration:**
   ```csharp
   modelBuilder.ApplyConfiguration(new OrderConfiguration());
   ```

3. **Add Permissions to Provider:**
   ```csharp
   OrdersPermissions.View,
   OrdersPermissions.Create,
   OrdersPermissions.Edit,
   OrdersPermissions.Delete,
   ```

4. **Run Database Migration:**
   ```bash
   dotnet ef migrations add AddOrderEntity
   dotnet ef database update
   ```

## 🎯 Architecture Support

- **Clean Architecture** with dependency inversion
- **CQRS** with MediatR for command/query separation  
- **Domain-Driven Design** patterns
- **Entity Framework Core** for data persistence
- **ASP.NET Core** Web API
- **FluentValidation** for input validation
- **Permission-based authorization**

## 🛠️ Troubleshooting

**Command not found?** Ensure .NET tools are in PATH
**Permission errors?** Run with elevated privileges
**Template errors?** Run `qcli doctor --fix`

Get help: `qcli [command] --help`

## 🤝 Contributing

This is a private tool for QuillySOFT projects. Contact the development team for access and contribution guidelines.

## 📄 License

Private software - All rights reserved by QuillySOFT.
