# QuillySOFT CLI

A powerful CLI tool for generating CRUD operations in CLIO architecture projects.

## Installation

### From NuGet (Private Feed)
```bash
dotnet tool install --global QuillySOFT.CLI --add-source https://your-private-nuget-feed.com/v3/index.json
```

### From Source
```bash
git clone https://github.com/QuillySoft/QCli
cd qcli/src/Apps/Tools.Cli
dotnet pack
dotnet tool install --global --add-source ./nupkg QuillySOFT.CLI
```

## Usage

### Initialize Configuration
Before using the tool, initialize the configuration in your project:

```bash
cd /path/to/your/clio/project
qcli config init
```

This will create a `qcli.json` configuration file with auto-detected paths.

### Generate CRUD Operations

#### Generate all CRUD operations for an entity:
```bash
qcli add Order --all
```

#### Generate specific operations:
```bash
qcli add Order --create --read
qcli add Product --update --delete
```

#### Generate with custom entity type:
```bash
qcli add Order --all --entity-type FullyAudited
```

### Configuration Management

#### Show current configuration:
```bash
qcli config show
```

#### Generate sample configuration:
```bash
qcli config sample
```

#### Set configuration values:
```bash
qcli config set --key rootpath --value "c:\projects\MyProject"
```

## Configuration File

The tool uses a `qcli.json` configuration file to define project paths and settings. Here's a sample:

```json
{
  "projectType": "CLIO",
  "paths": {
    "rootPath": "c:\\projects\\MyProject",
    "apiPath": "src\\Apps\\Api",
    "applicationPath": "src\\Core\\Application",
    "domainPath": "src\\Core\\Domain",
    "persistencePath": "src\\Infra\\Persistence",
    "applicationTestsPath": "tests\\Application\\ApplicationTests",
    "integrationTestsPath": "tests\\Infra\\InfraTests\\Controllers",
    "controllersPath": "src\\Apps\\Api\\Controllers"
  },
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": false,
    "generateMappingProfiles": false,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

## What Gets Generated

When you run `qcli add Order --all`, the tool generates:

### Domain Layer
- `Order.cs` - Domain entity
- `OrdersPermissions.cs` - Permission constants

### Application Layer
- **Commands:**
  - `CreateOrderCommand.cs` + Handler + Validator
  - `UpdateOrderCommand.cs` + Handler + Validator
  - `DeleteOrderCommand.cs` + Handler + Validator
- **Queries:**
  - `GetOrdersQuery.cs` + Handler (paginated list)
  - `GetOrderByIdQuery.cs` + Handler
- **Events:** (if enabled)
  - `OrderCreatedEvent.cs`
  - `OrderUpdatedEvent.cs`
  - `OrderDeletedEvent.cs`

### Infrastructure Layer
- `OrderConfiguration.cs` - Entity Framework configuration

### API Layer
- `OrdersController.cs` - REST API controller

### Tests
- Unit tests for all commands and queries
- Integration tests for the controller

## Command Options

### `add` command options:
- `--all, -a` - Generate all CRUD operations
- `--create, -c` - Generate create operation only
- `--read, -r` - Generate read operations only
- `--update, -u` - Generate update operation only
- `--delete, -d` - Generate delete operation only
- `--entity-type, -e` - Specify entity type (Audited or FullyAudited)

### `config` command options:
- `init` - Initialize configuration file
- `show` - Display current configuration
- `set` - Set a configuration value
- `get` - Get a configuration value
- `sample` - Generate sample configuration file

## Manual Steps After Generation

After running the tool, you'll need to complete these manual steps:

1. **Add DbSet to DbContext:**
   ```csharp
   // In ITenantDbContext and TenantDbContext
   public DbSet<Order> Orders { get; set; }
   ```

2. **Register Entity Configuration:**
   ```csharp
   // In TenantDbContext.OnModelCreating
   modelBuilder.ApplyConfiguration(new OrderConfiguration());
   ```

3. **Add Permissions to Provider:**
   ```csharp
   // In PermissionsProvider
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

## Architecture Support

Currently supports the **CLIO architecture pattern** with:
- Clean Architecture principles
- CQRS with MediatR
- Domain-Driven Design
- Entity Framework Core
- ASP.NET Core Web API
- FluentValidation
- Permission-based authorization

## Contributing

This is a private tool for QuillySOFT projects. Contact the development team for access and contribution guidelines.

## License

Private software - All rights reserved by QuillySOFT.
