# Code Generation Settings

The code generation settings in QCLI control what gets generated when you create entities and operations. These settings allow you to customize the output to match your project requirements and development preferences.

## Configuration Section

```json
{
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

## Setting Details

### Default Entity Type

**Property:** `defaultEntityType`  
**Type:** `string`  
**Default:** `"Audited"`  
**Valid Values:** `"Simple"`, `"Audited"`, `"BaseEntity"`

Controls the base type for generated entities.

```json
{
  "codeGeneration": {
    "defaultEntityType": "Audited"
  }
}
```

**Entity Type Inheritance:**
- **Simple**: No base class, plain entity
- **Audited**: Includes created/modified timestamps and user tracking
- **BaseEntity**: Basic entity with Id property

**Generated Entity Example (Audited):**
```csharp
public sealed class Product : AuditedEntity
{
    public string Name { get; private set; }
    
    private Product() { } // EF Core constructor
    
    public Product(Guid id, string name) : base(id)
    {
        Name = name;
    }
}
```

### Generate Events

**Property:** `generateEvents`  
**Type:** `boolean`  
**Default:** `true`

Controls whether domain events are generated and published in commands.

```json
{
  "codeGeneration": {
    "generateEvents": true
  }
}
```

**When Enabled:**
- Adds event publishing to command handlers
- Includes `IPublisher publisher` in handler constructors
- Generates event classes in `Events` folder
- Publishes events after entity operations

**Generated Command with Events:**
```csharp
public sealed class Handler(IApplicationDbContext dbContext, IPublisher publisher) 
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(Guid.NewGuid(), request.ProductDto.Name);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        // Event publishing when enabled
        await publisher.Publish(new ProductCreatedEvent(product.Id), cancellationToken);
        
        return product.Id;
    }
}
```

### Generate Mapping Profiles

**Property:** `generateMappingProfiles`  
**Type:** `boolean`  
**Default:** `true`

Controls whether Mapster mapping profiles are generated for DTOs.

```json
{
  "codeGeneration": {
    "generateMappingProfiles": true
  }
}
```

**When Enabled:**
- Creates mapping profile classes in `Mapping` folder
- Configures entity-to-DTO mappings
- Handles complex property mappings
- Supports both list and detail DTOs

**Generated Mapping Profile:**
```csharp
public sealed class ProductMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Product, ProductForListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);

        config.ForType<Product, ProductForReadDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
    }
}
```

### Generate Permissions

**Property:** `generatePermissions`  
**Type:** `boolean`  
**Default:** `true`

Controls whether permission constants and authorization attributes are generated.

```json
{
  "codeGeneration": {
    "generatePermissions": true
  }
}
```

**When Enabled:**
- Creates permission constants in Domain layer
- Adds `[Authorize]` attributes to commands
- Generates view, create, update, delete permissions
- Follows consistent permission naming patterns

**Generated Permissions:**
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

**Command with Authorization:**
```csharp
[Authorize(Permissions = [Permissions.Products.Actions.Create])]
public sealed class CreateProductCommand(ProductForCreateUpdateDto productDto) : IRequest<Guid>
{
    // Command implementation
}
```

### Generate Tests

**Property:** `generateTests`  
**Type:** `boolean`  
**Default:** `true`

Controls whether unit test files are generated for commands and queries.

```json
{
  "codeGeneration": {
    "generateTests": true
  }
}
```

**When Enabled:**
- Creates test directory structure
- Generates test files for each operation
- Includes both unit and integration test placeholders
- Follows testing best practices

**Test Directory Structure:**
```
tests/
├── Application.UnitTests/
│   └── Products/
│       ├── Commands/
│       │   ├── CreateProductCommandTests.cs
│       │   ├── UpdateProductCommandTests.cs
│       │   └── DeleteProductCommandTests.cs
│       └── Queries/
│           └── ProductQueryTests.cs
└── Application.IntegrationTests/
    └── Products/
        └── ProductIntegrationTests.cs
```

## Command Line Override

You can override these settings when using the `add` command:

```bash
# Skip specific generation features
qcli add product --skip-tests --skip-permissions

# Use different entity type
qcli add product --entity-type Simple

# Template override might include generation settings
qcli add product --template minimal
```

## Configuration Commands

### Set Individual Settings

```bash
# Set default entity type
qcli config set entitytype "Simple"

# View current settings
qcli config show
```

### Get Specific Setting

```bash
# Check current entity type
qcli config get entitytype
```

## Environment-Specific Configuration

Different environments might need different generation settings:

**Development (quillysoft-cli.dev.json):**
```json
{
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

**Production (quillysoft-cli.prod.json):**
```json
{
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": true,
    "generateTests": false
  }
}
```

## Best Practices

### Development Phase

- **Enable All Features**: Start with all generation options enabled
- **Use Audited Entities**: Most business entities benefit from audit tracking
- **Generate Tests**: Ensure comprehensive test coverage from the start

```json
{
  "codeGeneration": {
    "defaultEntityType": "Audited",
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": true,
    "generateTests": true
  }
}
```

### Production Optimization

- **Keep Core Features**: Events, mappings, and permissions remain important
- **Consider Test Strategy**: Production deployments might skip test generation
- **Maintain Consistency**: Use same entity types across environments

### Microservices

For microservice architectures, consider:

```json
{
  "codeGeneration": {
    "defaultEntityType": "Simple",
    "generateEvents": true,
    "generateMappingProfiles": true,
    "generatePermissions": false,
    "generateTests": true
  }
}
```

### Legacy Integration

When integrating with existing systems:

```json
{
  "codeGeneration": {
    "defaultEntityType": "BaseEntity",
    "generateEvents": false,
    "generateMappingProfiles": true,
    "generatePermissions": false,
    "generateTests": true
  }
}
```

## Impact on Generated Files

Each setting affects which files and code sections are generated:

| Setting | Affects | Generated Elements |
|---------|---------|-------------------|
| `defaultEntityType` | Domain entities | Base class inheritance |
| `generateEvents` | Commands, Domain | Event classes, publisher calls |
| `generateMappingProfiles` | Application | Mapster configuration |
| `generatePermissions` | Commands, Domain | Authorization attributes, constants |
| `generateTests` | Tests | Test files, directory structure |

## Validation

QCLI validates code generation settings:

- **Entity Type**: Must be valid type name
- **Boolean Settings**: Must be true/false
- **Consistency**: Checks for conflicting configurations

**Common Validation Errors:**
```bash
❌ Invalid entity type: "Invalid"
❌ generateEvents must be boolean value
❌ Cannot disable permissions with authorization templates
```

## Integration with Commands

Code generation settings integrate with various QCLI commands:

### Add Command

The `add` command respects all generation settings:

```bash
# Uses configuration settings
qcli add product

# Override specific settings
qcli add product --skip-tests --entity-type Simple
```

### Doctor Command

The `doctor` command validates generation settings:

```bash
qcli doctor
# Checks for:
# - Valid entity type values
# - Consistent permission settings
# - Test directory structure
```

### Config Command

Manage generation settings:

```bash
# Initialize with defaults
qcli config init

# Set specific values
qcli config set generateEvents false

# View all settings
qcli config show
```

## Troubleshooting

### Common Issues

**Missing Generated Files:**
- Check if feature is enabled in configuration
- Verify path configuration is correct
- Ensure proper permissions for file creation

**Compilation Errors:**
- Verify entity type is available in project
- Check if required dependencies are installed
- Ensure mapping profiles are registered

**Permission Issues:**
- Verify permission constants are generated
- Check authorization configuration
- Ensure permissions are registered in DI

### Debug Commands

```bash
# Check current configuration
qcli config show

# Validate configuration
qcli doctor

# View generation plan
qcli add product --dry-run
```

## Advanced Customization

For advanced scenarios, you can:

1. **Custom Entity Types**: Define your own base entity classes
2. **Selective Generation**: Enable/disable features per entity
3. **Template Customization**: Create templates with different generation patterns
4. **Plugin Integration**: Extend generation with custom plugins

See [Advanced Customization](../advanced/customization.md) for detailed guidance.
